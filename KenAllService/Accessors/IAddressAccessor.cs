namespace KenAllService.Accessors;

using Smart.Data.Accessor.Attributes;
using Smart.Data.Accessor.Builders;

[DataAccessor]
public interface IAddressAccessor
{
    [Execute]
    void Create();

    [Insert]
    void Insert(DbTransaction tx, AddressEntity entity);

    [Query]
    ValueTask<List<AddressEntity>> Query(string zipCode);
}
