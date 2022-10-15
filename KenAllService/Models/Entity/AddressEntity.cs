namespace KenAllService.Models.Entity;

public class AddressEntity
{
    public string JisX0402 { get; set; } = default!;

    public string OldZipCode { get; set; } = default!;

    public string ZipCode { get; set; } = default!;

    public string Kana1 { get; set; } = default!;

    public string Kana2 { get; set; } = default!;

    public string Kana3 { get; set; } = default!;

    public string Address1 { get; set; } = default!;

    public string Address2 { get; set; } = default!;

    public string Address3 { get; set; } = default!;

    // 2つ以上の郵便番号で1町域
    public int IsPartial { get; set; }

    // 番地が小字毎
    public int IsAddressPerSection { get; set; }

    // 丁目あり
    public int HasBlock { get; set; }

    // 1つの郵便番号で2つ以上
    public int IsMultiple { get; set; }

    public int UpdateStatus { get; set; }

    public int UpdateReason { get; set; }
}
