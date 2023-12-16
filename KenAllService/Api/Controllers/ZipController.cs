namespace KenAllService.Api.Controllers;

using KenAllService.Models.Api;
using KenAllService.Services;

using Microsoft.AspNetCore.Mvc;

using Smart.AspNetCore.Filters;

public sealed class ZipController : BaseApiController
{
    private AddressService AddressService { get; }

    public ZipController(AddressService addressService)
    {
        AddressService = addressService;
    }

    [HttpGet]
    public async ValueTask<IActionResult> Search([FromQuery] string zipCode)
    {
        return Ok(new ZipSearchResponse
        {
            Results = (await AddressService.QueryAsync(zipCode)).ToArray()
        });
    }

    [HttpPost]
    [ReadableBodyStream]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async ValueTask<IActionResult> Update()
    {
        if (Request.ContentLength == 0)
        {
            return BadRequest();
        }

        await AddressService.ImportAsync(Request.Body);

        return Ok();
    }
}
