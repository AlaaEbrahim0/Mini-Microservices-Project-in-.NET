using Microsoft.AspNetCore.Mvc;

namespace CommandService.Controllers;
[ApiController]
[Route("api/c/platforms")]
public class PlatformController : ControllerBase
{
    [HttpPost]
    public IActionResult TestInBoundConnection()
    {
        Console.WriteLine("--> Inbound POST # Command Service");
        return Ok("Inbound test of from Platform Cot");
    }
}
