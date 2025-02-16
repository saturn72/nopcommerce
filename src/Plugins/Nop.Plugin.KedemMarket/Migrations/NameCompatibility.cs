namespace KedemMarket.Migrations;

public partial class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new()
    {
        {typeof(NavbarInfo), "km_navbarinfo" },
        {typeof(NavbarElement), "km_navbarelement" },
        {typeof(NavbarElementVendor), "km_navbarelementvendor" },
        {typeof(KmOrder), "km_order" },
        {typeof(KmUserCustomerMap), "km_usercustomermap" },
    };

    public Dictionary<(Type, string), string> ColumnName => new();
}