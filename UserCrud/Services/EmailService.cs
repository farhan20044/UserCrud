using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using UserCrud.Services.Interfaces;

namespace UserCrud.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpSection = _config.GetSection("Smtp");
            var smtpClient = new SmtpClient(smtpSection["Host"])
            {
                Port = int.Parse(smtpSection["Port"]),
                Credentials = new NetworkCredential(smtpSection["Username"], smtpSection["Password"]),
                EnableSsl = bool.Parse(smtpSection["EnableSsl"])
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpSection["Username"]),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string confirmationLink)
        {
            var subject = "Confirm your email";
            var message = $@"<p>Please confirm your email by clicking the link below:</p><p><a href='{confirmationLink}'>Confirm Email</a></p>";
            await SendEmailAsync(toEmail, subject, message);
        }
    }
} 