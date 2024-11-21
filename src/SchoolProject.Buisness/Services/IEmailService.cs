using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Buisness.Services
{
    public interface IEmailService
    {
        Task SendEmail(string toEmail, string subject, string body);
    }
}