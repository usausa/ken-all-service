namespace KenAllService.Models.Api;

#pragma warning disable CA1819
public class ZipSearchResponse
{
    public AddressEntity[] Results { get; set; } = default!;
}
#pragma warning restore CA1819
