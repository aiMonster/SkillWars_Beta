﻿using Common.DTO.Communication;
using Common.DTO.Lobbie;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Services
{
    public interface ILobbieService
    {
        Task<LobbieInfo> CreateLobbieAsync(LobbieDTO lobbie);
        Task<List<LobbieInfo>> GetLobbiesAsync();
        Task<Response<List<TeamInfo>>> GetPlayersAsync(int lobbieId);
        Task<Response<bool>> ParticipateTeam(int userId, int teamId, string password);
        Task<Response<bool>> LeaveTeam(int userId, int teamId);
    }
}
