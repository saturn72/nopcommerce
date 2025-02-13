namespace KedemMarket.Infrastructure;

public partial class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new()
    {
        {typeof(NavbarInfo), "km_navbarinfo" },
        {typeof(NavbarElement), "km_navbarelement" },
        {typeof(NavbarElementVendor), "km_navbarelementvendor" },
    };

    public Dictionary<(Type, string), string> ColumnName => new();
}