namespace KenAllService.Api;

using Microsoft.AspNetCore.Mvc;

[Area("api")]
[Route("[area]/[controller]/[action]")]
[ApiController]
public abstract class BaseApiController : ControllerBase
{
}
