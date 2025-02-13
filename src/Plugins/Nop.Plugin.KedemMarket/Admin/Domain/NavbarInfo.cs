namespace KedemMarket.Admin.Domain;
public class NavbarInfo : BaseEntity, IStoreMappingSupported
{
    public IEnumerable<NavbarElement> Elements { get; set; }
    public int DisplayOrder { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Published { get; set; }
    public bool Deleted { get; set; }
    public bool LimitedToStores { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime UpdatedOnUtc { get; set; }
}

