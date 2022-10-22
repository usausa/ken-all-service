namespace KenAllService.Services;

using System.Collections.Generic;
using System.IO.Compression;

using CsvHelper;
using CsvHelper.Configuration;

using KenAllService.Accessors;
using KenAllService.Application.Csv;

using Smart.Data;
using Smart.Data.Accessor;

public class AddressService
{
    private readonly SemaphoreSlim sync = new(1, 1);

    private readonly ILogger<AddressService> log;

    private readonly IDbProvider dbProvider;

    private readonly IAddressAccessor addressAccessor;

    public AddressService(
        ILogger<AddressService> log,
        IDbProvider dbProvider,
        IAccessorResolver<IAddressAccessor> addressAccessor)
    {
        this.log = log;
        this.dbProvider = dbProvider;
        this.addressAccessor = addressAccessor.Accessor;
    }

    public async ValueTask<List<AddressEntity>> QueryAsync(string zipCode)
    {
        await sync.WaitAsync();
        try
        {
            return await addressAccessor.QueryAsync(zipCode);
        }
        finally
        {
            sync.Release();
        }
    }

    public async ValueTask ImportAsync(Stream stream)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        using var archive = new ZipArchive(ms);

        var success = 0;
        var failed = 0;

        await sync.WaitAsync();
        try
        {
            await addressAccessor.TruncateAsync();

            await using var con = dbProvider.CreateConnection();
            await con.OpenAsync();
            await using var tx = await con.BeginTransactionAsync();

            foreach (var entry in archive.Entries.Where(x => x.FullName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)))
            {
                using var reader = new StreamReader(entry.Open(), Encoding.GetEncoding("Shift_JIS"));
                using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    HasHeaderRecord = false
                });
                csv.Context.RegisterClassMap<AddressEntityMap>();
                while (await csv.ReadAsync().ConfigureAwait(false))
                {
                    // TODO Normalize
                    var record = csv.GetRecord<AddressEntity>();
                    if (record is null)
                    {
                        failed++;
                        continue;
                    }

                    await addressAccessor.InsertAsync(tx, record);
                    success++;
                }
            }

            await tx.CommitAsync();

            await addressAccessor.VacuumAsync();
        }
        finally
        {
            sync.Release();
        }

        log.LogInformation("Import completed. success=[{Success}], failed=[{Failed}]", success, failed);
    }
}
