using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("")]
    public class UserController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Get() => Ok("UserService is running!");
    }
}
