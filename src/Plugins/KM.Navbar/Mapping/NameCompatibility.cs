using Nop.Data.Mapping;

namespace KM.Navbar.Mapping
{
    public partial class NameCompatibility : INameCompatibility
    {
        public Dictionary<Type, string> TableNames => new()
        {
            {typeof(NavbarInfo), "km_navbarinfo" },
            {typeof(NavbarElement), "km_navbarelement" },
        };

        public Dictionary<(Type, string), string> ColumnName => new();
    }
}