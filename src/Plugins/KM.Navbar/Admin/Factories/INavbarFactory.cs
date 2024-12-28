namespace KM.Navbar.Admin.Factories;
public interface INavbarFactory
{
    Task PrepareNavbarInfoSearchModelAsync(NavbarInfoSearchModel searchModel);
    Task<NavbarInfoListModel> PrepareNavbarInfoListModelAsync(NavbarInfoSearchModel searchModel);
    Task<NavbarInfoModel> PrepareNavbarInfoModelAsync(NavbarInfoModel model, NavbarInfo navbarInfo, bool excludeProperties = false);
    Task<NavbarInfoElementListModel> PrepareNavbarInfoElementListModelAsync(NavbarElementSearchModel searchModel, NavbarInfo navbar);
    Task PrepareCreateNavbarElementPopupModelAsync(CreateOrUpdateNavbarElementPopupModel model);
}
