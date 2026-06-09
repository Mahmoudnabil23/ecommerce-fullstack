namespace E_Commerce_Backend.Services.EmailService
{
    public interface IEmailService
    {
        public Task SendEmailAsync(EmailMessage message);
    }
}
