using Common.DTO.Account;
using Common.Helpers;
using Common.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    
    public class UserEntity : IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string SteamId { get; set; }

        public string Email { get; set; }

        public string NickName { get; set; }
        public string PhoneNumber { get; set; }

        public string Password { get; set; }        

        public DateTime RegistrationDate { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public int Balance { get; set; }

        public List<UserTeamEntity> UserTeams { get; set; }
                

        public UserEntity()
        {
            UserTeams = new List<UserTeamEntity>();
        }

        public UserEntity(RegistrationDTO data)
        {
            Email = data.Email;
            NickName = data.Email.Split('@')[0];
            Password = TripleDESCryptHelper.Encript(data.Password);

            RegistrationDate = DateTime.Now;
            IsEmailConfirmed = false;
            PhoneNumber = data.PhoneNumber;
            UserTeams = new List<UserTeamEntity>();
        }

        public UserEntity(string steamId, string nickName)
        {
            NickName = nickName;
            SteamId = steamId;
            RegistrationDate = DateTime.Now;
            IsEmailConfirmed = false;
            UserTeams = new List<UserTeamEntity>();
        }       
    }
}
