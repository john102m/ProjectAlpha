using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.MessagingBaseClasses;

namespace UserService.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("")]
    public class UserController : ControllerBase
    {
        public IMessagePublisher _publisher { get; set; }
        public UserController(IMessagePublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpGet("health")]
        public IActionResult Get() => Ok("UserService is running!");
    }
}
