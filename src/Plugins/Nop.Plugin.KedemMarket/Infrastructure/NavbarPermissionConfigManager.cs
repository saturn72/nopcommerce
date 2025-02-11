//using Nop.Core.Domain.Security;
//using Nop.Services.Security;

//namespace KM.Navbar.Infrastructure;
//public sealed class NavbarPermissionConfigManager: IPermissionConfigManager
//{
//    public static readonly PermissionConfig NavbarListAdd = new()
//    {
//        Name = "Add Navbar",
//        SystemName = "NavbarListAdd",
//        Category = "Navbar"
//    };
//    public static readonly PermissionRecord NavbarListRead = new()
//    {
//        Name = "View Navbar list",
//        SystemName = "NavbarListRead",
//        Category = "Navbar"
//    };
//    public static readonly PermissionRecord NavbarListUpdate = new()
//    {
//        Name = "Update Navbar",
//        SystemName = "NavbarListUpdate",
//        Category = "Navbar"
//    };
//    public static readonly PermissionRecord NavbarListDelete = new()
//    {
//        Name = "Delete Navbar",
//        SystemName = "NavbarListDelete",
//        Category = "Navbar"
//    };
//    private List<PermissionConfig> _allConfig;

//    public IList<PermissionConfig> AllConfigs => _allConfig ??=new List<PermissionConfig>()
//        {
//            NavbarListAdd,
//            NavbarListRead, 
//            NavbarListUpdate, 
//            NavbarListDelete,
//        };
//    }
//    /// <summary>
//    /// Get default permissions
//    /// </summary>
//    /// <returns>Permissions</returns>
//    public virtual HashSet<(string systemRoleName, PermissionRecord[] permissions)> GetDefaultPermissions()
//    {
//        return new() { (NopCustomerDefaults.AdministratorsRoleName, new[] { ViewNavbars }) };
//    }
//}