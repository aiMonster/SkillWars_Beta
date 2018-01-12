using Common.DTO.Account;
using Common.DTO.Communication;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Interfaces.Services
{
    public interface IAccountService
    {
        Task<Response<OperationResults>> Register(RegistrationDTO data, CancellationToken token = new CancellationToken());
        Task<Response<UserDTO>> ConfirmEmail(string token);
        Task<Response<UserDTO>> RemoveUser(int id);

        Task<Response<UserDTO>> GetUserById(int userId);
        Task<Response<bool>> ChangePassword(ChangePasswordRequest request, int userId);
        Task<List<UserDTO>> GetUsers();
        Task<Response<bool>> RestorePassword(string email);
        Task<Response<bool>> RestorePasswordConfirm(string confirmationToken, string newPassword);
    }
}
