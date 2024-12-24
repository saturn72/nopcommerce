namespace KM.Navbar.Security;
public class NavbarPermissions
{
    public const string NAVBARS_VIEW = $"{nameof(NavbarInfo)}.NavbarsView";
    public const string NAVBARS_CREATE = $"{nameof(NavbarInfo)}.NavbarsCreate";
    public const string NAVBARS_EDIT = $"{nameof(NavbarInfo)}.NavbarsEdit";
    public const string NAVBARS_DELETE = $"{nameof(NavbarInfo)}.NavbarsDelete";

    public const string NAVBARS_ELEMENTS_VIEW = $"{nameof(NavbarElement)}.NavbarElementsView";
    public const string NAVBARS_ELEMENTS_CREATE = $"{nameof(NavbarElement)}.NavbarElementsCreate";
    public const string NAVBARS_ELEMENTS_EDIT = $"{nameof(NavbarElement)}.NavbarElementsEdit";
    public const string NAVBARS_ELEMENTS_DELETE = $"{nameof(NavbarElement)}.NavbarElementsDelete";
}