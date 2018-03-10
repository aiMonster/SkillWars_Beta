using Common.DTO.Communication;
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
        Task<LobbieInfo> CreateLobbieAsync(LobbieDTO lobbie, int userId);
        Task<List<LobbieInfo>> GetLobbiesAsync();
        Task<Response<TeamDTO>> GetPlayersAsync(int lobbieId);
        Task<Response<bool>> ParticipateTeam(int userId, ParticipateLobbieRequest request);
        Task<Response<bool>> LeaveTeam(int userId, int lobbieId);
        Task<Response<LobbieInfo>> RemoveLobbie(int lobbieId);
    }
}
