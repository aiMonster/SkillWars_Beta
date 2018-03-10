using Common.DTO.Communication;
using Common.DTO.Lobbie;
using Common.Entity;
using Common.Helpers;
using Common.Interfaces.Services;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Enums;
using System.Threading.Tasks;

using Newtonsoft.Json;
using System.Net;
using System.IO;
using Common.DTO.SocketServer;

namespace Services.LobbieService
{
    public class LobbieService : ILobbieService
    {
        private readonly MSContext _context;
       


        public LobbieService(MSContext context)
        {
            _context = context;            
        }

        public async Task<LobbieInfo> CreateLobbieAsync(LobbieDTO lobbie, int userId)
        {
            LobbieEntity entity = new LobbieEntity(lobbie);
            
            await _context.Lobbies.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            await _context.Teams.AddAsync(new TeamEntity(entity.Id) { Type = TeamTypes.FirstTeam });
            await _context.Teams.AddAsync(new TeamEntity(entity.Id) { Type = TeamTypes.SecondTeam });
            await _context.SaveChangesAsync();

            var team = await _context.Teams.Where(t => t.LobbieId == entity.Id && t.Type == TeamTypes.FirstTeam)
                                            .Include(p => p.Lobbie)
                                            .Include(p => p.UserTeams)
                                            .FirstOrDefaultAsync();

            team.UserTeams.Add(new UserTeamEntity() { UserId = userId, TeamId = team.Id, LobbieId = team.LobbieId });
            await _context.SaveChangesAsync();

            await SendNotifies(new NewLobbie(entity));
            return new LobbieInfo(entity);
        }

        public async Task<List<LobbieInfo>> GetLobbiesAsync() =>
            new List<LobbieInfo>(await _context.Lobbies.Select(e => new LobbieInfo(e)).ToListAsync());        

        public async Task<Response<TeamDTO>> GetPlayersAsync(int lobbieId)
        {
            var response = new Response<TeamDTO>() { Data = new TeamDTO() };
            
            if (await _context.Lobbies.AsNoTracking().FirstOrDefaultAsync(l => l.Id == lobbieId) == null)
            {
                response.Error = new Error(404, "Lobbie is not found");
                return response;
            }

            var teams = _context.Teams.Where(t => t.LobbieId == lobbieId).AsNoTracking(); 
            await teams.ForEachAsync(t =>
            {                
                response.Data.Teams.Add(new TeamInfo(t,
                    _context.UserTeams.Where(ut => ut.TeamId == t.Id)
                       .AsNoTracking()
                       .Include(ut => ut.User)
                       .Select(ut => ut.User).ToList()
                    ));
                response.Data.LobbieId = t.LobbieId;
            });     
             
            return response;
        }



        public async Task<Response<bool>> ParticipateTeam(int userId, ParticipateLobbieRequest request)
        {
            Response<bool> response = new Response<bool>();

            var started = await _context.UserTeams.AsNoTracking()
                                        .Include(ut => ut.Team)
                                            .ThenInclude(t => t.Lobbie)
                                        .Where(ut => ut.UserId == userId &&
                                                        (ut.Team.Lobbie.Status == LobbieStatusTypes.Expectation || ut.Team.Lobbie.Status == LobbieStatusTypes.Started))
                                        .ToListAsync();
            if(started.Any())
            {
                response.Error = new Error(203, "You alread participate in started lobbie Idiot!");
                return response;
            }

            var team = await _context.Teams.Where(t => t.LobbieId == request.LobbieId && t.Type == request.TeamType)
                                            .Include(p => p.Lobbie)
                                            .Include(p => p.UserTeams)
                                            .FirstOrDefaultAsync();

            
            if (team == null)
            {
                response.Error = new Error(404, "Team is not found");
                return response;
            }
            else if(team.Lobbie.Status != LobbieStatusTypes.Expectation)
            {
                response.Error = new Error(203, "Lobbie already started game or canceled or finished Idiot!");
                return response;
            }
            else if((await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)).Balance < team.Lobbie.Bet)
            {
                response.Error = new Error(203, "Not enought money Idiot!");
                return response;
            }
            else if(team.Lobbie.IsPrivate && (String.IsNullOrEmpty(request.Password) || team.Lobbie.Password != TripleDESCryptHelper.Encript(request.Password)))
            {
                response.Error = new Error(203, "Not correct password Idiot!");
                return response;
            } 
            else if(team.UserTeams.Count == team.Lobbie.AmountPlayers/2)
            {
                response.Error = new Error(203, "Team is full Idiot!");
                return response;
            }

            team.UserTeams.Add(new UserTeamEntity() { UserId = userId, TeamId = team.Id, LobbieId = team.LobbieId });
            await _context.SaveChangesAsync();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            await SendNotifies(new UserParticipated() { UserId = userId, TeamType = team.Type, LobbieId = team.LobbieId, NickName = user.NickName });

            response.Data = true;
            return response;
        }

        public async Task<Response<bool>> LeaveTeam(int userId, int lobbieId)
        {
            Response<bool> response = new Response<bool>();            

            var part = await _context.UserTeams
                                     .Where(u => u.LobbieId == lobbieId && u.UserId == userId)
                                     .Include(ut => ut.Team)
                                        .ThenInclude(t => t.Lobbie)
                                     .FirstOrDefaultAsync();
            if (part == null)
            {               
                response.Error = new Error(203, "stupid, you are not there");
                return response;
            }
            else if (part.Team.Lobbie.Status != LobbieStatusTypes.Expectation)
            {
                response.Error = new Error(203, "Lobbie already started game Idiot!");
                return response;
            }
            _context.UserTeams.Remove(part);           
            await _context.SaveChangesAsync();

            //var lobbie = await _context.Lobbies.Where(l => l.Id == part.Team.Lobbie.Id).Include(l => l.Teams).ThenInclude(t => t.UserTeams).FirstOrDefaultAsync();
            //if(lobbie.Teams[0].UserTeams.Count == 0 && lobbie.Teams[1].UserTeams.Count == 0)
            //{
            //    lobbie.Status = LobbieStatusTypes.Canceled;
            //    await _context.SaveChangesAsync();
            //}

            await SendNotifies(new UserLeaved() { UserId = userId, LobbieId = lobbieId });
            response.Data = true;
            return response;
        }

        public async Task<Response<LobbieInfo>> RemoveLobbie(int lobbieId)
        {
            

            var response = new Response<LobbieInfo>();
            var entity = _context.Lobbies.FirstOrDefault(l => l.Id == lobbieId);
            if(entity == null)
            {
                response.Error = new Error(404, "Lobbie not found");
                return response;
            }
            _context.Lobbies.Remove(entity);
            await _context.SaveChangesAsync();
            response.Data = new LobbieInfo(entity);

            await SendNotifies(new RemovedLobbie() { LobbieId = entity.Id});
            return response;
        }

        private async Task SendNotifies(object data)
        {
            string message = JsonConvert.SerializeObject(data);
            Console.WriteLine(message);
            try
            {
                WebRequest request = WebRequest.Create("http://localhost:8080/api/Lobbie");
                request.Method = "POST";
                var body = System.Text.Encoding.UTF8.GetBytes(message);
                request.ContentType = "application/json";
                request.ContentLength = body.Length;


                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(body, 0, body.Length);
                    stream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    response.Close();
                }

                Console.WriteLine("Запрос выполнен...");
            }
            catch
            {
                Console.WriteLine("Something wrong while connecting to the localhost");
            }

        }

    }
}
