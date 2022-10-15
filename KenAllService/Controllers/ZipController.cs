namespace KenAllService.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class ZipController : ControllerBase
{
    [HttpGet]
    public IActionResult Search()
    {
        return Ok();
    }
}
