using Microsoft.AspNetCore.Mvc;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("EmailService is running!");
    }
}
