namespace KenAllService.Api;

using Microsoft.AspNetCore.Mvc;

[Area("api")]
[Microsoft.AspNetCore.Mvc.Route("[area]/[controller]/[action]")]
[ApiController]
public class BaseApiController : ControllerBase
{
}
