using KedemMarket.Admin.Models.Navbar;

namespace KedemMarket.Admin.Factories;
public interface INavbarFactory
{
    Task PrepareNavbarInfoSearchModelAsync(NavbarInfoSearchModel searchModel);
    Task<NavbarInfoListModel> PrepareNavbarInfoListModelAsync(NavbarInfoSearchModel searchModel);
    Task<NavbarInfoModel> PrepareNavbarInfoModelAsync(NavbarInfoModel model, NavbarInfo navbarInfo, bool excludeProperties = false);
    Task<NavbarInfoElementListModel> PrepareNavbarInfoElementListModelAsync(NavbarElementSearchModel searchModel, NavbarInfo navbar);
    Task PrepareCreateOrUpdateNavbarElementModelAsync(CreateOrUpdateNavbarElementModel model);
    Task PrepareAddVendorToNavbarElementModel(AddVendorToNavbarElementSearchModel searchModel);
    Task<NavbarElementVendorListModel> PrepareNavbarElementVendorListSearchModelAsync(NavbarElementVendorListSearchModel searchModel);
    Task<VendorAddPopupListModel> VendorAddPopupListAsync(NavbarElementVendorListSearchModel searchModel);
}
