using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Account
{
    public class RegistrationDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }        

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }        

        //[Required]
        //public string PhoneNumber { get; set; }
    }
}
