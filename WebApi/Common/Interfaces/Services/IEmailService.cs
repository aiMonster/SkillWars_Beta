using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendMail(string email, string message, string subject, string filePath = null);

        Task SendMail(List<string> email, string message, string subject, string filePath = null);
    }
}
