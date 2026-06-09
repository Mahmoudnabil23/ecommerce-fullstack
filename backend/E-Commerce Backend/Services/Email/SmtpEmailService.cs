using E_Commerce_Backend.Services.EmailService;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Runtime;

namespace E_Commerce_Backend.Services.Email
{
    public class SmtpEmailService : IEmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public SmtpEmailService(IOptions<SmtpSettings> smtpSettings)
        {
                _smtpSettings = smtpSettings.Value;
        }
        public async Task SendEmailAsync(EmailMessage message)
        {
            // 1. Create the envelope and letter (MimeKit)
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(message.To));
            email.Subject = message.Subject;

            // For now, we are just sending raw HTML/Text. We will add templates later!
            var builder = new BodyBuilder { HtmlBody = message.Body };
            email.Body = builder.ToMessageBody();

            // 2. Drive to the post office and drop it off (MailKit)
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
                await client.SendAsync(email);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}
