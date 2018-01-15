#define release

using Common.DTO.Account;
using Common.DTO.Communication;
using Common.Entity;
using Common.Enums;
using Common.Helpers;
using Common.Interfaces.Services;
using DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly MSContext _context;
        private readonly IEmailService _emailService;
        private readonly IConfigurationRoot _config;

        public AccountService(MSContext context, IEmailService emailService, IConfigurationRoot config)
        {
            _context = context;
            _emailService = emailService;
            _config = config;
        }

        public async Task<Response<OperationResults>> Register(RegistrationDTO data, CancellationToken token = new CancellationToken())
        {
            var response = new Response<OperationResults>()
            {
                Data = OperationResults.Failed
            };

            var isEmailAlreadyUsed = await _context.Users.AnyAsync(p => p.Email == data.Email);
            if (isEmailAlreadyUsed)
            {

                response.Error = new Error(400, "This email is already used");
                return response;
            }

            var user = new UserEntity(data);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync(token);

            response.Data = OperationResults.Success;

            var confirmationToken = Guid.NewGuid().ToString();
            await _context.Tokens.AddAsync(new TokenEntity
            {
                UserId = user.Id,
                Id = confirmationToken,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            });
            await _context.SaveChangesAsync();

            #if release
            var apiPath = _config["AppLinks:frontPath"] + "confirm-register?token=" + confirmationToken;
            var link = "<a href='" + apiPath + "'>link</a>";

            await _emailService.SendMail(user.Email, _config["Register"] + link, "Registration");
            #endif
            return response;
        }

        public async Task<Response<UserDTO>> ConfirmEmail(string confirmationToken)
        {
            var response = new Response<UserDTO>();

            var token = await _context.Tokens.Where(p => p.Id == confirmationToken)
                .Include(p => p.User).FirstOrDefaultAsync();

            if (token == null)
            {
                response.Error = new Error(400, "Token is not valid");
                return response;
            }

            if (token.User == null)
            {
                response.Error = new Error(404, "User not found");
                return response;
            }

            if (token.ExpirationDate < DateTime.UtcNow)
            {
                _context.Tokens.Remove(token);
                response.Error = new Error(400, "Confirmation date is over");
                return response;
            }

            if (token.User.IsEmailConfirmed)
            {
                response.Error = new Error(403, "Email is already confirmed");
                return response;
            }

            token.User.IsEmailConfirmed = true;
            _context.Tokens.Remove(token);
            await _context.SaveChangesAsync();
            #if release
            await _emailService.SendMail(token.User.Email, _config["EmailConfirmed"], "Email confirmed");
            #endif
            response.Data = new UserDTO(token.User);
            return response;
        }

        public async Task<Response<UserDTO>> RemoveUser(int id)
        {
            var response = new Response<UserDTO>();

            var user = await _context.Users.FirstOrDefaultAsync(p => p.Id == id);

            if (user == null)
            {
                response.Error = new Error(400, "User not found");
                return response;
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            response.Data = new UserDTO(user);
            return response;
        }

        public async Task<Response<UserDTO>> GetUserById(int id)
        {
            var response = new Response<UserDTO>();

            var user = await _context.Users.FirstOrDefaultAsync(p => p.Id == id);

            if (user == null)
            {
                response.Error = new Error(400, "User not found");
                return response;
            }
            response.Data = new UserDTO(user);
            return response;
        }

        public async Task<List<UserDTO>> GetUsers()
        {
            return await _context.Users.Select(n => new UserDTO(n)).ToListAsync();
        }

        public async Task<Response<bool>> ChangePassword(ChangePasswordRequest request, int userId)
        {
            Response<bool> response = new Response<bool>();

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                response.Error = new Error(404, "User not found");
                return response;
            }

            if (request.NewPassword != request.ConfirmPassword)
            {
                response.Error = new Error(400, "Confirm password is not correct");
                return response;
            }

            if (TripleDESCryptHelper.Encript(request.OldPassword) != user.Password)
            {
                response.Error = new Error(403, "Not correct password");
                return response;
            }

            user.Password = TripleDESCryptHelper.Encript(request.NewPassword);
            await _context.SaveChangesAsync();
#if release
            //await _sendingService.Send(new ReceiverContainer(user), _config["ChangePassword"]);
            await _emailService.SendMail(user.Email, _config["ChangePassword"], "Changing password");
#endif
            response.Data = true;
            return response;
        }

        public async Task<Response<bool>> RestorePassword(string email)
        {
            Response<bool> response = new Response<bool>();

            var user = await _context.Users.FirstOrDefaultAsync(p => p.Email == email);
            if (user == null)
            {
                response.Error = new Error(404, "User not found");
                return response;
            }

            var confirmationToken = Guid.NewGuid().ToString();
            await _context.Tokens.AddAsync(new TokenEntity
            {
                UserId = user.Id,
                Id = confirmationToken,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            });
            await _context.SaveChangesAsync();

#if release
            var apiPath = _config["AppLinks:frontPath"] + "restore-password?token=" + confirmationToken;
            var link = "<a href='" + apiPath + "'>link</a>";

            await _emailService.SendMail(user.Email, _config["RestorePassword"] + link, "Restore Password");
            //await _sendingService.Send(new ReceiverContainer(user), _config["RestorePassword"] + link);
#endif
            response.Data = true;
            return response;
        }

        //public async Task<Response<bool>> RestorePassword(string email)
        //{
        //    Response<bool> response = new Response<bool>();

        //    var user = await _context.Users.FirstOrDefaultAsync(p => p.Email == email);
        //    if (user == null)
        //    {
        //        response.Error = new Error(404, "User not found");
        //        return response;
        //    }

        //    int GetRandomNumber()
        //    {
        //        Random rnd = new Random();
        //        return rnd.Next(100000, 999999);                
        //    }

        //    int gotNumber = GetRandomNumber();
        //    while(true)
        //    {               
        //        if(await _context.ConfirmationPasswords.FirstOrDefaultAsync(c => c.Value == gotNumber) == null)
        //        {
        //            break;
        //        }
        //        gotNumber = GetRandomNumber();
        //    }

        //    await _context.ConfirmationPasswords.AddAsync(new ConfirmationPasswordEntity
        //    {
        //        Value = gotNumber,
        //        ExpirationDate = DateTime.UtcNow.AddHours(1),
        //        UserId = user.Id

        //    });            
        //    await _context.SaveChangesAsync();

        //    #if release
        //    var apiPath = _config["AppLinks:frontPath"] + "/#/restore-password;token=" + gotNumber;
        //    var link = "<a href='" + apiPath + "'>link</a>";

        //    await _emailService.SendMail(email, "Restore password", _config["RestorePassword"] + link);
        //    #endif

        //    response.Data = true;
        //    return response;
        //}

        //public async Task<Response<bool>> RestorePasswordConfirm(int confirmationPassword, string newPassword)
        //{
        //    Response<bool> response = new Response<bool>();

        //    var token = await _context.ConfirmationPasswords.Where(p => p.Value == confirmationPassword)
        //           .Include(p => p.User).FirstOrDefaultAsync();

        //    if (token == null)
        //    {
        //        response.Error = new Error(400, "Code is not valid");
        //        return response;
        //    }

        //    if (token.User == null)
        //    {
        //        response.Error = new Error(404, "User not found");
        //        return response;
        //    }

        //    if (token.ExpirationDate < DateTime.UtcNow)
        //    {
        //        _context.ConfirmationPasswords.Remove(token);
        //        response.Error = new Error(400, "Confirmation date is over");
        //        return response;
        //    }

        //    token.User.Password = TripleDESCryptHelper.Encript(newPassword);
        //    _context.ConfirmationPasswords.Remove(token);
        //    await _context.SaveChangesAsync();

        //    #if release
        //    await _emailService.SendMail(token.User.Email, "Password restoring confirmation", _config["ChangePassword"]);
        //    #endif

        //    response.Data = true;
        //    return response;
        //}

        public async Task<Response<bool>> RestorePasswordConfirm(string confirmationToken, string newPassword)
        {
            Response<bool> response = new Response<bool>();

            var token = await _context.Tokens.Where(p => p.Id == confirmationToken)
                   .Include(p => p.User).FirstOrDefaultAsync();

            if (token == null)
            {
                response.Error = new Error(400, "Token is not valid");
                return response;
            }

            if (token.User == null)
            {
                response.Error = new Error(404, "User not found");
                return response;
            }

            if (token.ExpirationDate < DateTime.UtcNow)
            {
                _context.Tokens.Remove(token);
                response.Error = new Error(400, "Confirmation date is over");
                return response;
            }

            token.User.Password = TripleDESCryptHelper.Encript(newPassword);
            _context.Tokens.Remove(token);
            await _context.SaveChangesAsync();

#if release
            //await _sendingService.Send(new ReceiverContainer(token.User), _config["ChangePassword"]);
            await _emailService.SendMail(token.User.Email, _config["ChangePassword"], "Restore Password");
#endif
            response.Data = true;
            return response;
        }
    }
}
