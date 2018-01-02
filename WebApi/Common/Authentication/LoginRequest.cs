using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Authentication
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Login { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}
