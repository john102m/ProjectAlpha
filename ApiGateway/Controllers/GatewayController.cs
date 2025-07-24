using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GatewayController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Get() => Ok("ApiGateway is running!");
    }
}
