﻿using Google.Cloud.Firestore;
using KedemMarket.Services.Documents;

namespace KedemMarket.Documents;

[FirestoreData]
public record UserProfileDocument : IDocument
{
    [FirestoreProperty]
    public string id { get; set; }
    [FirestoreProperty]
    public string userId { get; set; }
    [FirestoreProperty]
    public AddressDocument billingInfo { get; set; }
}
