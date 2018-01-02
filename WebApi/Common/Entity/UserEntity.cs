using Common.DTO.Account;
using Common.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entity
{
    //*T add interface implementation
    public class UserEntity //:IUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }        

        public DateTime RegistrationDate { get; set; }

        public bool IsEmailConfirmed { get; set; }
                

        public UserEntity() { }

        public UserEntity(RegistrationDTO data)
        {
            Email = data.Email;
            Password = TripleDESCryptHelper.Encript(data.Password);

            RegistrationDate = DateTime.Now;
            IsEmailConfirmed = false;
            
        }

        //public string NickName { get; set; }
        //public string PhoneNumber { get; set; }
        //public DateTime BirthDate { get; set; }
    }
}
