using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ProviderAPI.Controllers
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Healthy");
    }
}
