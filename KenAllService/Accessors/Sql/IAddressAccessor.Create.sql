CREATE TABLE Address (
    JisX0402 text,
    OldZipCode text,
    ZipCode text,
    Kana1 text,
    Kana2 text,
    Kana3 text,
    Address1 text,
    Address2 text,
    Address3 text,
    IsPartial int,
    IsAddressPerSection int,
    HasBlock int,
    IsMultiple int,
    UpdateStatus int,
    UpdateReason int);

CREATE INDEX IX_Address ON Address (ZipCode);
