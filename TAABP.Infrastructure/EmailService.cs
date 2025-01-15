using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using TAABP.Application;

namespace TAABP.Infrastructure
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var smtpConfig = _configuration.GetSection("SMTP");

            using (var client = new SmtpClient(smtpConfig["Host"], int.Parse(smtpConfig["Port"])))
            {
                client.Credentials = new NetworkCredential(smtpConfig["UserName"], smtpConfig["Password"]);
                client.EnableSsl = bool.Parse(smtpConfig["EnableSSL"]);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpConfig["UserName"], "Your App Name"),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
