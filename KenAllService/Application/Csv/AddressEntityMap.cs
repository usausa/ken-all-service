namespace KenAllService.Application.Csv;

using CsvHelper.Configuration;

public sealed class AddressEntityMap : ClassMap<AddressEntity>
{
    public AddressEntityMap()
    {
        Map(x => x.JisX0402).Index(0);
        Map(x => x.OldZipCode).Index(1);
        Map(x => x.ZipCode).Index(2);
        Map(x => x.Kana1).Index(3);
        Map(x => x.Kana2).Index(4);
        Map(x => x.Kana3).Index(5);
        Map(x => x.Address1).Index(6);
        Map(x => x.Address2).Index(7);
        Map(x => x.Address3).Index(8);
        Map(x => x.IsPartial).Index(9);
        Map(x => x.IsAddressPerSection).Index(10);
        Map(x => x.HasBlock).Index(11);
        Map(x => x.IsMultiple).Index(12);
        Map(x => x.UpdateStatus).Index(13);
        Map(x => x.UpdateReason).Index(14);
    }
}
