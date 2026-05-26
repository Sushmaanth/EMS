using EMSAuthApi.Models;
using EMSAuthApi.Services.Abstraction;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace EMSAuthApi.Services
{
    public class EmailService: IEmailService
    {
        private readonly EmailSettings settings;

        public EmailService(IConfiguration configuration)
        {
            settings = configuration.GetSection("EmailSettings").Get<EmailSettings>();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            //Creates email object.
            var email = new MimeMessage(); 

            email.From.Add(MailboxAddress.Parse(settings.Email));

            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                settings.Host,
                settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                settings.Email,
                settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
