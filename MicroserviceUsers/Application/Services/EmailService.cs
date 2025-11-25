using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;        

namespace Application.Services
{
    public class EmailService(IConfiguration config) : IEmailService
    {
        private readonly IConfiguration _config = config;

        public async Task SendAsync(string to, string subject, string body)
        {
            var mailSettings = _config.GetSection("Email");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("Inno Shop", mailSettings["From"]));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new SmtpClient();

            var port = int.Parse(mailSettings["Port"]);
            var host = mailSettings["Host"];
            var from = mailSettings["From"];
            var password = mailSettings["Password"];
            var useSsl = bool.Parse(mailSettings["UseSsl"]);

            await smtp.ConnectAsync(host, port, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(from, password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }


}
