using Microsoft.AspNetCore.Mvc;
using Shared.Messaging.Infrastructure.RabbitMq;

namespace UserService.Controllers
{
    [ApiController]
    [Route("")]
    public class UserController(IMessagePublisher publisher) : ControllerBase
    {
        public IMessagePublisher _publisher { get; set; } = publisher;

        [HttpGet("health")]
        public async Task<IActionResult> Get()
        {
            var message = new BaseMessage
            {
                Id = 119,
                Username = "A message from the User Controller: Health method; All systems operational!",
                MessageType = $"TEST MESSAGE TYPE",
                CreatedAt = DateTime.Now,
            };

            await _publisher.PublishMessageAsync(message);

            return Ok("UserService is running!");
        }
    }
}
