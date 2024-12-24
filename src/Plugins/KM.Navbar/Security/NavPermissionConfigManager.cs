
using Nop.Services.Security;

namespace KM.Navbar.Security;

public partial class NavPermissionConfigManager : IPermissionConfigManager
{
    public IList<PermissionConfig> AllConfigs => new List<PermissionConfig>
    {
        new ("Admin area. Navbars view list", NavbarPermissions.NAVBARS_VIEW, nameof(NavbarInfo), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Navbars create navbar", NavbarPermissions.NAVBARS_CREATE, nameof(NavbarInfo), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Navbars update navbar", NavbarPermissions.NAVBARS_EDIT, nameof(NavbarInfo), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Navbars delete navbar", NavbarPermissions.NAVBARS_DELETE, nameof(NavbarInfo), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Navbar elements view navbar",NavbarPermissions.NAVBARS_ELEMENTS_VIEW, nameof(NavbarElement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Navbar elements create navbar",NavbarPermissions.NAVBARS_ELEMENTS_CREATE, nameof(NavbarElement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Navbar elements update navbar",NavbarPermissions.NAVBARS_ELEMENTS_EDIT, nameof(NavbarElement), NopCustomerDefaults.AdministratorsRoleName),
        new ("Admin area. Navbar elements delete navbar",NavbarPermissions.NAVBARS_ELEMENTS_DELETE, nameof(NavbarElement), NopCustomerDefaults.AdministratorsRoleName),
    };
}
