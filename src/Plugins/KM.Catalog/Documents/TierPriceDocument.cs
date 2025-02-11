﻿using Google.Cloud.Firestore;

namespace KedemMarket.Catalog.Documents;

[FirestoreData]
public record TierPriceDocument
{
    [FirestoreProperty("quantity")]
    public int Quantity { get; set; }
    
    [FirestoreProperty("price")]
    public float Price { get; set; }
    
    [FirestoreProperty("startDateTimeUtc")]
    public DateTime? StartDateTimeUtc { get; set; }
    
    [FirestoreProperty("endDateTimeUtc")]
    public DateTime? EndDateTimeUtc { get; set; }
}
