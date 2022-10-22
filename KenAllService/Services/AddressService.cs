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

    private IDbProvider DbProvider { get; }

    private IAddressAccessor AddressAccessor { get; }

    public AddressService(
        IDbProvider dbProvider,
        IAccessorResolver<IAddressAccessor> addressAccessor)
    {
        DbProvider = dbProvider;
        AddressAccessor = addressAccessor.Accessor;
    }

    public async ValueTask<List<AddressEntity>> QueryAsync(string zipCode)
    {
        await sync.WaitAsync();
        try
        {
            return await AddressAccessor.QueryAsync(zipCode);
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

        await sync.WaitAsync();
        try
        {
            await AddressAccessor.TruncateAsync();

            await using var con = DbProvider.CreateConnection();
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
                        continue;
                    }

                    await AddressAccessor.InsertAsync(tx, record);
                }
            }

            await tx.CommitAsync();

            await AddressAccessor.VacuumAsync();
        }
        finally
        {
            sync.Release();
        }
    }
}
