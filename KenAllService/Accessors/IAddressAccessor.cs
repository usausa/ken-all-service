namespace KenAllService.Accessors;

using Smart.Data.Accessor.Attributes;
using Smart.Data.Accessor.Builders;

[DataAccessor]
public interface IAddressAccessor
{
    [Execute]
    void Create();

    [Execute]
    ValueTask TruncateAsync();

    [Execute]
    ValueTask VacuumAsync();

    [Insert]
    ValueTask InsertAsync(DbTransaction tx, AddressEntity entity);

    [Query]
    ValueTask<List<AddressEntity>> QueryAsync(string zipCode);
}
