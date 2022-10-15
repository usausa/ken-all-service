namespace KenAllService.Api.Controllers;

using KenAllService.Accessors;
using KenAllService.Models.Api;

using Microsoft.AspNetCore.Mvc;

using Smart.Data.Accessor;

public class ZipController : BaseApiController
{
    private IAddressAccessor AddressAccessor { get; }

    public ZipController(IAccessorResolver<IAddressAccessor> addressAccessor)
    {
        AddressAccessor = addressAccessor.Accessor;
    }

    [HttpGet]
    public async ValueTask<IActionResult> Search([FromQuery] string zipCode)
    {
        return Ok(new ZipSearchResponse
        {
            Results = (await AddressAccessor.Query(zipCode)).ToArray()
        });
    }

    // TODO Import
}
