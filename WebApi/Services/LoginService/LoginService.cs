using Common.Authentication;
using Common.DTO.Communication;
using Common.DTO.Steam;
using Common.Entity;
using Common.Helpers;
using Common.Interfaces.Entities;
using Common.Interfaces.Services;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
//using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.LoginService
{
    public class LoginService : ILoginService
    {
        private readonly MSContext _context;
        private readonly string _ApiKey;

        public LoginService(MSContext context, IConfigurationRoot root)
        {
            _context = context;
            _ApiKey = root.GetSection("Steam")["api_key"];
        }

        public async Task<TokenResponse> GetToken(ClaimsIdentity identity)
        {
            TokenResponse response = new TokenResponse();

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

            response = tokenResponse;
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

        public async Task<Response<ClaimsIdentity>> GetIdentity(string steamId)
        {
            Response<ClaimsIdentity> response = new Response<ClaimsIdentity>();

            string userName = "";
            using (var client = new HttpClient())
            {
                // Query steam user summary endpoint
                var resp = await client.GetAsync($"http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={_ApiKey}&steamids={steamId}");

                if(resp.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    response.Error = new Error(500, "Server is brocken, we need to update api key");
                    return response;
                }
                              
                var players  = JsonConvert.DeserializeObject<SteamPlayerSummaryRootObject>(await resp.Content.ReadAsStringAsync()).Response.Players; 
                if(players.Count == 0)
                {
                    response.Error = new Error(404, "User with such id is not found on the Steam");
                    return response;
                }
                userName = players[0].PersonaName;
            }

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(p => p.SteamId == steamId);
            if (user == null)
            {
                _context.Users.Add(new UserEntity(steamId, userName));
                await _context.SaveChangesAsync();
                response.Error = new Error(202, "Need to add email");
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
