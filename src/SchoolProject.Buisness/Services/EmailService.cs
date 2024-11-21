using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SchoolProject.Buisness.Services
{
    public class EmailService:IEmailService
    {
        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var fromEmail = ""; 
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("", ""),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromEmail, toEmail, subject, body);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
