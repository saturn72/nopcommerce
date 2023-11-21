﻿namespace Km.Catalog.Documents;

[FirestoreData]
public record TierPriceDocument
{
    [FirestoreProperty]
    public int quantity { get; set; }
    [FirestoreProperty]
    public float price { get; set; }
    [FirestoreProperty]
    public DateTime? startDateTimeUtc { get; set; }
    [FirestoreProperty]
    public DateTime? endDateTimeUtc { get; set; }
}
