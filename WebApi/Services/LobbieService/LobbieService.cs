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

namespace Services.LobbieService
{
    public class LobbieService : ILobbieService
    {
        private readonly MSContext _context;

        public LobbieService(MSContext context)
        {
            _context = context;
        }

        public async Task<LobbieInfo> CreateLobbieAsync(LobbieDTO lobbie)
        {
            LobbieEntity entity = new LobbieEntity(lobbie);
            
            await _context.Lobbies.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            await _context.Teams.AddAsync(new TeamEntity(entity.Id));
            await _context.Teams.AddAsync(new TeamEntity(entity.Id));
            await _context.SaveChangesAsync();

            return new LobbieInfo(entity);
        }

        public async Task<List<LobbieInfo>> GetLobbiesAsync() =>
            new List<LobbieInfo>(await _context.Lobbies.Select(e => new LobbieInfo(e)).ToListAsync());        

        public async Task<Response<List<TeamInfo>>> GetPlayersAsync(int lobbieId)
        {
            var response = new Response<List<TeamInfo>>() { Data = new List<TeamInfo>() };
            
            if (await _context.Lobbies.AsNoTracking().FirstOrDefaultAsync(l => l.Id == lobbieId) == null)
            {
                response.Error = new Error(404, "Lobbie is not found");
                return response;
            }

            var teams = _context.Teams.Where(t => t.LobbieId == lobbieId).AsNoTracking(); 
            await teams.ForEachAsync(t =>
            {                
                response.Data.Add(new TeamInfo(t,
                    _context.UserTeams.Where(ut => ut.TeamId == t.Id)
                       .AsNoTracking()
                       .Include(ut => ut.User)
                       .Select(ut => ut.User).ToList()
                    ));                
            });            
            return response;
        }

        public async Task<Response<bool>> ParticipateTeam(int userId, int teamId, string password)
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

            var team = await _context.Teams.Where(t => t.Id == teamId)
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
                response.Error = new Error(203, "Lobbie already started game Idiot!");
                return response;
            }
            else if(team.Lobbie.IsPrivate && (String.IsNullOrEmpty(password) || team.Lobbie.Password != TripleDESCryptHelper.Encript(password)))
            {
                response.Error = new Error(203, "Not correct password Idiot!");
                return response;
            } 

            team.UserTeams.Add(new UserTeamEntity() { UserId = userId, TeamId = teamId });
            await _context.SaveChangesAsync();

            response.Data = true;
            return response;
        }

        public async Task<Response<bool>> LeaveTeam(int userId, int teamId)
        {
            Response<bool> response = new Response<bool>();            

            var part = await _context.UserTeams
                                     .Where(u => u.TeamId == teamId && u.UserId == userId)
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
            response.Data = true;
            return response;
        }
    }
}
