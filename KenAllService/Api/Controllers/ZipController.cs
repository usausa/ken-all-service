namespace KenAllService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

public class ZipController : BaseApiController
{
    [HttpGet]
    public IActionResult Search()
    {
        return Ok();
    }
}
