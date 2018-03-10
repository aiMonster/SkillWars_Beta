using Common.Authentication;
using Common.DTO.Communication;
using Common.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Services
{
    public interface ILoginService
    {
        Task<IUser> GetUserByCreds(string login, string password);

        Task<Response<ClaimsIdentity>> GetIdentity(string login, string password);
        Task<Response<ClaimsIdentity>> GetIdentity(string steamId);

        Task<TokenResponse> GetToken(ClaimsIdentity identity);
    }
}
