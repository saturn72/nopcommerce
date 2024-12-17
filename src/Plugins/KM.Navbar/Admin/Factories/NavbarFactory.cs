
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Discounts;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace KM.Navbar.Admin.Factories;

public class NavbarFactory : INavbarFactory
{
    private readonly INavbarInfoService _navbarService;
    private readonly ILocalizationService _localizationService;
    protected readonly IBaseAdminModelFactory _baseAdminModelFactory;

    public NavbarFactory(
        INavbarInfoService navbarService,
        ILocalizationService localizationService,
        IBaseAdminModelFactory baseAdminModelFactory)
    {
        _navbarService = navbarService;
        _localizationService = localizationService;
        _baseAdminModelFactory = baseAdminModelFactory;
    }

    public async Task PrepareNavbarInfoSearchModelAsync(NavbarInfoSearchModel searchModel)
    {
        searchModel.AvailablePageSizes = "10, 50, 100";
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

        var model = new NavbarInfoListModel().PrepareToGrid(searchModel, navbars, () => navbars.Select(nb => ToModel(nb)));

        return model;
    }

    public Task<NavbarInfoModel> PrepareNavbarInfoModelAsync(NavbarInfoModel model, NavbarInfo navbarInfo, bool excludeProperties = false)
    {
        if (navbarInfo != null)
        {
            //fill in model values from the entity
            if (model == null)
                model = ToModel(navbarInfo);

            PrepareCategoryProductSearchModel(model.CategoryProductSearchModel, navbarInfo);
        }

        //set default values for the new model
        if (navbarInfo == null)
        {
            model.PageSize = _catalogSettings.DefaultCategoryPageSize;
            model.PageSizeOptions = _catalogSettings.DefaultCategoryPageSizeOptions;
            model.Published = true;
            model.IncludeInTopMenu = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PriceRangeFiltering = true;
            model.ManuallyPriceRange = true;
            model.PriceFrom = NopCatalogDefaults.DefaultPriceRangeFrom;
            model.PriceTo = NopCatalogDefaults.DefaultPriceRangeTo;
        }

        model.PrimaryStoreCurrencyCode = (await _currencyService.GetCurrencyByIdAsync(_currencySettings.PrimaryStoreCurrencyId)).CurrencyCode;

        //prepare localized models
        if (!excludeProperties)
            model.Locales = await _localizedModelFactory.PrepareLocalizedModelsAsync(localizedModelConfiguration);

        //prepare available category templates
        await _baseAdminModelFactory.PrepareCategoryTemplatesAsync(model.AvailableCategoryTemplates, false);

        //prepare available parent categories
        await _baseAdminModelFactory.PrepareCategoriesAsync(model.AvailableCategories,
            defaultItemText: await _localizationService.GetResourceAsync("Admin.Catalog.Categories.Fields.Parent.None"));

        //prepare model discounts
        var availableDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToCategories, showHidden: true, isActive: null);
        await _discountSupportedModelFactory.PrepareModelDiscountsAsync(model, navbarInfo, availableDiscounts, excludeProperties);

        //prepare model stores
        await _storeMappingSupportedModelFactory.PrepareModelStoresAsync(model, navbarInfo, excludeProperties);

        return model;
    }
}
private NavbarInfoModel ToModel(NavbarInfo navbar)
{
    return new()
    {
        Id = navbar.Id,
        Elements = navbar.Elements.Select(n => ToModel(n)).ToList(),
        DisplayOrder = navbar.DisplayOrder,
        Name = navbar.Name,
        Published = navbar.Published
    };
}

private NavbarElementModel ToModel(NavbarElement navbarElement)
{
    return new()
    {
        Alt = navbarElement.Alt,
        Icon = navbarElement.Icon,
        Id = navbarElement.Id,
        Index = navbarElement.Index,
        Label = navbarElement.Label,
        Tags = navbarElement.Tags,
        Type = navbarElement.Type.ToString().ToLower(),
        Value = navbarElement.Value,
    };
}

}
