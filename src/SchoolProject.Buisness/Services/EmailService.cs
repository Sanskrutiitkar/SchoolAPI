
using System.Net;
using System.Net.Mail;


namespace SchoolProject.Buisness.Services
{
    public class EmailService:IEmailService
    {
        public async Task SendEmail(string toEmail, string subject, string body)
        {
            var fromEmail = "fromEmail"; 
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("fromEmail", "password"),
                EnableSsl = true
            };

            var mailMessage = new MailMessage(fromEmail, toEmail, subject, body);
            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
