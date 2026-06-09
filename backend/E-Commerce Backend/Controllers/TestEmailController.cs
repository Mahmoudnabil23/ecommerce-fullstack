using E_Commerce_Backend.Services.EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestEmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendTestEmail()
        {
            var msg = new EmailMessage
            {
                To = "moatazmohammed2392003@gmail.com",
                Subject = "Hello from the new Ecommerce app!",
                Body = "<h1>Success!</h1><p>The .NET architecture is working.</p>"
            };

            await _emailService.SendEmailAsync(msg);
            return Ok("Email sent successfully!");
        }
    }
}
