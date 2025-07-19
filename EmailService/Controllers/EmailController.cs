using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("")]
    public class EmailController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Get() => Ok("EmailService is running!");
    }
}
