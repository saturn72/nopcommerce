
using Nop.Services.Security;

namespace KM.Navbar.Security;

public partial class NavPermissionConfigManager : IPermissionConfigManager
{
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        new ("Admin area. Navbars view list", NavbarPermissions.NAVBARS_VIEW, nameof(NavbarInfo), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Navbars create or update navbar", NavbarPermissions.NAVBARS_VIEW, nameof(NavbarInfo), NopCustomerDefaults.AdministratorsRoleName),
    };

}
