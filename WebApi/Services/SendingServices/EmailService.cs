using Common.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Services.SendingServices
{
    public class EmailService : IEmailService
    {
        private readonly string _email;
        private readonly string _password;
        private readonly string _host;

        public EmailService(IConfigurationRoot root)
        {
            var config = root.GetSection("SmtpConfig");
            _email = config["email"];
            _password = config["password"];
            _host = config["host"];
        }

        public async Task SendMail(string email, string message, string subject, string filePath = null)
        {
            await SendMail(new List<string> { email }, message, subject, filePath);
        }

        /*T*/
        public async Task SendMail(List<string> emails, string message, string subject, string filePath = null)
        {
            using (MailMessage Message = new MailMessage()
            {
                Subject = subject,
                Body = message,
                BodyEncoding = Encoding.GetEncoding("utf-8"),
                From = new MailAddress(_email),
                IsBodyHtml = true,
            })
            {
                foreach (var email in emails)
                {
                    Message.To.Add(new MailAddress(email));
                }

                if (filePath != null)
                {
                    Message.Attachments.Add(new Attachment(filePath));
                }

                using (SmtpClient Smtp = new SmtpClient()
                {
                    Host = _host,
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(_email, _password)
                })
                {
                    Smtp.Send(Message);
                }
            }
        }
    }
}
