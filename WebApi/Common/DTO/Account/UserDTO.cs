using Common.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Account
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string SteamId { get; set; }
        public string Email { get; set; }
        public string NickName { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public int Balance { get; set; }
        public string PhoneNumber { get; set; }

        public UserDTO() { }

        public UserDTO(UserEntity entity)
        {
            Id = entity.Id;
            SteamId = entity.SteamId;
            Email = entity.Email;
            NickName = entity.NickName;
            IsEmailConfirmed = entity.IsEmailConfirmed;
            Balance = entity.Balance;
            PhoneNumber = entity.PhoneNumber;
        }
    }
}
