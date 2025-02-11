﻿namespace KedemMarket.Catalog.Domain;

public class KmStoresSnapshot : BaseEntity
{
    public string? Data { get; set; }
    public uint Version { get; set; }
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
}
