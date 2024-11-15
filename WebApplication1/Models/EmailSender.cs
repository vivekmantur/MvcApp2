using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;
namespace WebApplication1.Models
{
    public class EmailSender : IEmailSender
    {
        private readonly string _smtpServer = "smtp.yourserver.com"; // Replace with actual SMTP
        private readonly string _smtpUser = "yourEmail@domain.com";
        private readonly string _smtpPass = "yourPassword";

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_smtpServer)
            {
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true
            };
            return client.SendMailAsync(new MailMessage(_smtpUser, email, subject, message));
        }
    }

}
