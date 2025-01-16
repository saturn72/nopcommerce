using KM.Navbar.Admin.Models;
using Nop.Services.Vendors;

namespace KM.Navbar.Admin.Factories;

public class NavbarFactory : INavbarFactory
{
    private readonly INavbarService _navbarService;
    private readonly ILocalizationService _localizationService;
    private readonly IBaseAdminModelFactory _baseAdminModelFactory;
    private readonly NavbarSettings _navbarSettings;
    private readonly IVendorService _vendorService;

    public NavbarFactory(
        INavbarService navbarService,
        ILocalizationService localizationService,
        IBaseAdminModelFactory baseAdminModelFactory,
        NavbarSettings navbarSettings,
        IVendorService vendorService)
    {
        _navbarService = navbarService;
        _localizationService = localizationService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _navbarSettings = navbarSettings;
        _vendorService = vendorService;
    }

    public async Task PrepareNavbarInfoSearchModelAsync(NavbarInfoSearchModel searchModel)
    {
        searchModel.AvailablePageSizes = _navbarSettings.PageSizeOptions;

        searchModel.AvailablePublishedOptions = new List<SelectListItem>(new[]{
            new SelectListItem
        {
            Value = "0",
            Text = await _localizationService.GetResourceAsync("Admin.Navbar.List.SearchPublished.All")
        },
            new SelectListItem
        {
            Value = "1",
            Text = await _localizationService.GetResourceAsync("Admin.Navbar.List.SearchPublished.PublishedOnly")
        },new SelectListItem
        {
            Value = "2",
            Text = await _localizationService.GetResourceAsync("Admin.Navbar.List.SearchPublished.UnpublishedOnly")
        }
        });

        await _baseAdminModelFactory.PrepareStoresAsync(searchModel.AvailableStores);
        searchModel.SetGridPageSize();
    }
    public virtual async Task<NavbarInfoListModel> PrepareNavbarInfoListModelAsync(NavbarInfoSearchModel searchModel)
    {
        ThrowIfNull(searchModel);

        var navbars = await _navbarService.GetAllNavbarInfosAsync(
            navbarName: searchModel.SearchNavbarName,
            showHidden: true,
            storeId: searchModel.SearchStoreId,
            pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize,
            overridePublished: searchModel.SearchPublishedId == 0 ? null : searchModel.SearchPublishedId == 1);

        var model = new NavbarInfoListModel().PrepareToGrid(searchModel, navbars, () => navbars.Select(nb => nb.ToModel<NavbarInfoModel>()));

        return model;
    }
    public async Task<NavbarInfoModel> PrepareNavbarInfoModelAsync(NavbarInfoModel model, NavbarInfo navbarInfo, bool excludeProperties = false)
    {
        if (navbarInfo != null)
        {
            //fill in model values from the entity
            if (model == null)
                model = navbarInfo.ToModel<NavbarInfoModel>();
        }
        model ??= new NavbarInfoModel();
        //set default values for the new model
        if (navbarInfo == null)
        {
            model.NavbarElementSearchModel = new()
            {
                PageSizeOptions = _navbarSettings.PageSizeOptions,
                AllowCustomersToSelectPageSize = true,
            };
            model.Published = true;
        }

        await _baseAdminModelFactory.PrepareStoresAsync(model.AvailableStores);

        //await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, navbarInfo, excludeProperties);

        return model;
    }
    public async Task<NavbarInfoElementListModel> PrepareNavbarInfoElementListModelAsync(NavbarElementSearchModel searchModel, NavbarInfo navbarInfo)
    {
        ThrowIfNull(searchModel, nameof(searchModel));
        ThrowIfNull(navbarInfo, nameof(navbarInfo));

        var navbarElements = await _navbarService.GetNavbarElementsByNavbarInfoIdAsync(
            navbarInfo.Id,
            pageIndex: searchModel.Page - 1,
            pageSize: searchModel.PageSize);

        //prepare grid model
        var model = new NavbarInfoElementListModel();
        return model.PrepareToGrid(searchModel, navbarElements,
            () => navbarElements.Select(navbarElement => navbarElement.ToModel<NavbarElementModel>()));
    }
    public Task PrepareCreateOrUpdateNavbarElementModelAsync(CreateOrUpdateNavbarElementModel model)
    {
        model.AvailableTypes ??= new List<SelectListItem>
        {
            new()
            {
                Selected = true,
                Text = Consts.NavbarElementType.Route,
                Value = Consts.NavbarElementType.Route.ToLower(),
            },new()
            {
                Selected = false,
                Text = Consts.NavbarElementType.Filter,
                Value = Consts.NavbarElementType.Filter.ToLower(),
            },
        };
        model.VendorSearchModel ??= new()
        {
            SearchNavbarElementId = model.NavbarElementId
        };

        return Task.CompletedTask;
    }

    #region vendor
    public async Task<NavbarElementVendorListModel> PrepareNavbarElementVendorListSearchModelAsync(NavbarElementVendorListSearchModel searchModel)
    {
        ThrowIfNull(searchModel);
        var pageIndex = Math.Max(searchModel.Page - 1, 0);
        var nevs = await _navbarService.GetNavbarElementVendorsByNavbarElementIdAsync(searchModel.SearchNavbarElementId, pageIndex, searchModel.PageSize)
            ?? throw new ArgumentException("No navbar element vendors found with the specified id");
        var vendors = await _vendorService.GetAllVendorsAsync();

        foreach (var n in nevs)
        {
            var cur = vendors.First(x => x.Id == n.VendorId);
            n.Vendor = cur;
        }

        var model = new NavbarElementVendorListModel().PrepareToGrid(searchModel, nevs, () => nevs.Select(nb => nb.ToModel<NavbarElementVendorModel>()));
        return model;
    }
    public async Task PrepareAddVendorToNavbarElementModel(AddVendorToNavbarElementSearchModel searchModel)
    {
        ThrowIfNull(searchModel);

        await _baseAdminModelFactory.PrepareVendorsAsync(searchModel.AvailableVendors);
        var nevs = await _navbarService.GetNavbarElementVendorsByNavbarElementIdAsync(searchModel.NavbarElementId);
        foreach (var nev in nevs)
        {
            var vendor = searchModel.AvailableVendors.First(v => v.Value == nev.VendorId.ToString());
            if (vendor != null)
                vendor.Selected = true;
        }
        searchModel.SelectedVendorIds = nevs.Select(nev => nev.VendorId).ToList();

        searchModel.SetPopupGridPageSize();
    }
    public async Task<VendorAddPopupListModel> VendorAddPopupListAsync(NavbarElementVendorListSearchModel searchModel)
    {
        ThrowIfNull(searchModel);

        var maps = await _navbarService.GetNavbarElementVendorsByNavbarElementIdAsync(searchModel.SearchNavbarElementId)
            ?? throw new ArgumentException("No navbar element vendors found with the specified id");

        var vendors = await _vendorService.GetAllVendorsAsync();

        var model = new VendorAddPopupListModel().PrepareToGrid(searchModel, vendors,
            () => vendors.Select(nb => new NavbarElementVendorSelectModel
            {
                VendorId = nb.Id,
                VendorName = nb.Name,
            }));

        model.SelectedVendorIds = maps.Select(nev => nev.VendorId).ToList();
        return model;
    }

    #endregion
}
