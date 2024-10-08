using Microsoft.AspNetCore.Mvc;
using MegStore.Application.Services;
using System.Threading.Tasks;
using MegStore.Core.Interfaces;
using Microsoft.AspNetCore.RateLimiting;

namespace MegStore.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("fixed")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ToEmail) ||
                string.IsNullOrWhiteSpace(request.Subject) ||
                string.IsNullOrWhiteSpace(request.Message))
            {
                return BadRequest("Invalid email data.");
            }

            await _emailService.SendEmailAsync(request.ToEmail, request.Subject, request.Message);
            return Ok("Email sent successfully.");
        }
    }

    public class EmailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
    }
}
