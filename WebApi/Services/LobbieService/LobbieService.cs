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

        public async Task<LobbieInfo> CreateLobbie(LobbieDTO lobbie)
        {
            LobbieEntity entity = new LobbieEntity(lobbie);
            
            await _context.Lobbies.AddAsync(entity);
            await _context.SaveChangesAsync();

            //List<TeamEntity> teams = new List<TeamEntity>
            //{
            //    new TeamEntity(entity.Id),
            //    new TeamEntity(entity.Id)
            //};

            await _context.Teams.AddAsync(new TeamEntity(entity.Id));
            await _context.Teams.AddAsync(new TeamEntity(entity.Id));
            await _context.SaveChangesAsync();

            return new LobbieInfo(entity);
        }

        public async Task<List<LobbieInfo>> GetLobbies()
        {
            return await _context.Lobbies.Select(e => new LobbieInfo(e)).ToListAsync();
        }

        public async Task<Response<List<TeamInfo>>> GetPlayers(int lobbieId)
        {
            var response = new Response<List<TeamInfo>>();
            response.Data = new List<TeamInfo>();
            //List<TeamInfo> l = new List<TeamInfo>();

            var teams = _context.Teams.Where(t => t.LobbieId == lobbieId).AsNoTracking();

            await teams.ForEachAsync(p =>
            {
                var users = _context.UserTeams.Where(u => u.TeamId == p.Id)
                       .AsNoTracking()
                       .Include(a => a.User)
                       .Select(b => b.User).ToList();

                //l.Add(new TeamInfo(p, users));
                response.Data.Add(new TeamInfo(p, users));
                
            });

            return response;
        }

        public async Task<Response<bool>> ParticipateTeam(int userId, int teamId, string password)
        {
            Response<bool> response = new Response<bool>();            
            var team = await _context.Teams.Where(t => t.Id == teamId).Include(p => p.Lobbie).Include(p => p.UserTeams).FirstOrDefaultAsync();
            
            if (team == null)
            {
                response.Error = new Error(404, "notFound");
                return response;
            }
            else if(team.Lobbie.IsPrivate &&(String.IsNullOrEmpty(password) || team.Lobbie.Password != TripleDESCryptHelper.Encript(password)))
            {
                response.Error = new Error(404, "not correct password idiot");
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

            var part = await _context.UserTeams.Where(u => u.TeamId == teamId && u.UserId == userId).FirstOrDefaultAsync();
            if (part == null)
            {               
                response.Error = new Error(203, "stupid, you are not there");
                return response;
            }

            _context.UserTeams.Remove(part);           
            await _context.SaveChangesAsync();
            response.Data = true;
            return response;
        }
    }
}
