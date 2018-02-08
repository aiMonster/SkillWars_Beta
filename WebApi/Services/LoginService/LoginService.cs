using Common.Authentication;
using Common.DTO.Communication;
using Common.Entity;
using Common.Helpers;
using Common.Interfaces.Entities;
using Common.Interfaces.Services;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
//using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.LoginService
{
    public class LoginService : ILoginService
    {
        private readonly MSContext _context;

        public LoginService(MSContext context)
        {
            _context = context;
        }

        public async Task<Response<TokenResponse>> GetToken(ClaimsIdentity identity)
        {
            Response<TokenResponse> response = new Response<TokenResponse>();

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                    issuer: AuthOptions.ISSUER,
                    audience: AuthOptions.AUDIENCE,
                    notBefore: now,
                    claims: identity.Claims,
                    expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                    signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var tokenResponse = new TokenResponse
            {
                Token = encodedJwt,
                UserId = identity.Name,
                NickName = identity.RoleClaimType
                
            };

            response.Data = tokenResponse;
            return response;
        }

        public async Task<Response<ClaimsIdentity>> GetIdentity(string login, string password)
        {
            Response<ClaimsIdentity> response = new Response<ClaimsIdentity>();

            var user = await GetUserByCreds(login, password);
            if (user == null)
            {
                response.Error = new Error(404, "Invalid username or password");
                return response;
            }

            if (!user.IsEmailConfirmed)
            {
                response.Error = new Error(403, "Email is not confirmed");
                return response;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString())
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity
                (claims, "Token", ClaimsIdentity.DefaultNameClaimType, user.NickName);

            response.Data = claimsIdentity;
            return response;
        }

        public async Task<IUser> GetUserByCreds(string login, string password)
        {
            var encrypted = TripleDESCryptHelper.Encript(password);
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(p => p.Email == login && p.Password == encrypted);           
            return user;
        }
    }
}
