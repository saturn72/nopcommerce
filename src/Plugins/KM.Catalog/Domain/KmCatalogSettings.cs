using Nop.Core.Configuration;

namespace KM.Catalog.Domain;

public class KmCatalogSettings : ISettings
{
    public int DefaultVendorId { get; set; }
}
