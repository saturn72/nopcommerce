namespace KM.Navbar.Admin.Factories;

public class NavbarFactory : INavbarFactory
{
    private readonly INavbarInfoService _navbarService;
    private readonly ILocalizationService _localizationService;
    private readonly IBaseAdminModelFactory _baseAdminModelFactory;
    private readonly NavbarSettings _navbarSettings;

    public NavbarFactory(
        INavbarInfoService navbarService,
        ILocalizationService localizationService,
        IBaseAdminModelFactory baseAdminModelFactory,
        NavbarSettings navbarSettings)
    {
        _navbarService = navbarService;
        _localizationService = localizationService;
        _baseAdminModelFactory = baseAdminModelFactory;
        _navbarSettings = navbarSettings;
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
        ArgumentNullException.ThrowIfNull(searchModel);

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

    public Task PrepareNavbarElemenModelAsync(NavbarElementModel model)
    {
        return Task.CompletedTask;
    }
}
