using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Entities
{
    public interface IUser
    {
        int Id { get; set; }

        string Email { get; set; }

        string NickName { get; set; }

        string Password { get; set; }

        DateTime RegistrationDate { get; set; }

        bool IsEmailConfirmed { get; set; }

        int Balance { get; set; }
    }
}
