using Nop.Web.Framework.Controllers;

namespace KM.Navbar.Admin.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin] //confirms access to the admin panel
[Area(AreaNames.ADMIN)] //specifies the area containing a controller or action
public class NavbarController : BaseAdminController
{
    private const string VIEW_PATH = "~/Plugins/KM.Navbar/Admin/Views/";
    private readonly INavbarFactory _navbarFactory;
    private readonly INavbarInfoService _navbarInfoService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreService _storeService;

    public NavbarController(
        INavbarFactory navbarFactory,
        INavbarInfoService navbarInfoService,
        INotificationService notificationService,
        ILocalizationService localizationService,
        IStoreMappingService storeMappingService,
        IStoreService storeService)
    {
        _navbarFactory = navbarFactory;
        _navbarInfoService = navbarInfoService;
        _notificationService = notificationService;
        _localizationService = localizationService;
        _storeMappingService = storeMappingService;
        _storeService = storeService;
    }

    public static string GetViewPath(string viewName) => VIEW_PATH + viewName;

    public override ViewResult View(string? viewName, object? model)
    {
        return base.View(GetViewPath(viewName), model);
    }

    public virtual IActionResult Index()
    {
        return RedirectToAction(nameof(List));
    }

    [CheckPermission(NavbarPermissions.NAVBARS_VIEW)]
    public virtual async Task<IActionResult> List()
    {
        var model = new NavbarInfoSearchModel();
        await _navbarFactory.PrepareNavbarInfoSearchModelAsync(model);
        return View("List.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_VIEW)]
    public virtual async Task<IActionResult> List(NavbarInfoSearchModel searchModel)
    {
        var model = await _navbarFactory.PrepareNavbarInfoListModelAsync(searchModel);
        return Json(model);
    }

    [CheckPermission(NavbarPermissions.NAVBARS_CREATE)]
    public virtual async Task<IActionResult> Create()
    {
        var model = await _navbarFactory.PrepareNavbarInfoModelAsync(new NavbarInfoModel(), null);
        return View("Create.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(NavbarPermissions.NAVBARS_CREATE)]
    public virtual async Task<IActionResult> Create(NavbarInfoModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var navbar = model.ToEntity<NavbarInfo>();
            navbar.CreatedOnUtc = DateTime.UtcNow;
            navbar.UpdatedOnUtc = DateTime.UtcNow;
            await _navbarInfoService.InsertNavbarAsync(navbar);

            var msg = await _localizationService.GetResourceAsync("Admin.Navbars.Added");
            _notificationService.SuccessNotification(msg);

            await SaveStoreMappingsAsync(navbar, model);
            if (!continueEditing || navbar.Id == 0)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = navbar.Id });
        }

        model = await _navbarFactory.PrepareNavbarInfoModelAsync(model, null, true);
        return View("Create.cshtml", model);
    }

    [CheckPermission(NavbarPermissions.NAVBARS_EDIT)]
    public virtual async Task<IActionResult> Edit(int id)
    {
        var navbar = await _navbarInfoService.GetNavbarInfoByIdAsync(id);
        if (navbar == null || navbar.Deleted)
            return RedirectToAction("List");

        //prepare model
        var model = await _navbarFactory.PrepareNavbarInfoModelAsync(null, navbar);

        return View("Edit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(NavbarPermissions.NAVBARS_EDIT)]
    public virtual async Task<IActionResult> Edit(NavbarInfoModel model, bool continueEditing)
    {
        //try to get a navbarinfo with the specified id
        var navbar = await _navbarInfoService.GetNavbarInfoByIdAsync(model.Id);
        if (navbar == null || navbar.Deleted)
            return RedirectToAction("List");

        if (ModelState.IsValid)
        {
            navbar.UpdatedOnUtc = DateTime.UtcNow;
            navbar.Name = model.Name;


            navbar = model.ToEntity(navbar);
            navbar.UpdatedOnUtc = DateTime.UtcNow;
            await _navbarInfoService.UpdateNavbarInfoAsync(navbar);

            await SaveStoreMappingsAsync(navbar, model);

            var msg = await _localizationService.GetResourceAsync("Admin.Navbars.Updated");
            _notificationService.SuccessNotification(msg);

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = navbar.Id });
        }

        model = await _navbarFactory.PrepareNavbarInfoModelAsync(model, navbar, true);
        return View("Edit.cshtml", model);
    }

    protected virtual async Task SaveStoreMappingsAsync(NavbarInfo navbarinfo, NavbarInfoModel model)
    {
        navbarinfo.LimitedToStores = model.SelectedStoreIds.Any();
        await _navbarInfoService.UpdateNavbarInfoAsync(navbarinfo);

        var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(navbarinfo);
        var allStores = await _storeService.GetAllStoresAsync();
        foreach (var store in allStores)
        {
            if (model.SelectedStoreIds.Contains(store.Id))
            {
                //new store
                if (!existingStoreMappings.Any(sm => sm.StoreId == store.Id))
                    await _storeMappingService.InsertStoreMappingAsync(navbarinfo, store.Id);
            }
            else
            {
                //remove store
                var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                if (storeMappingToDelete != null)
                    await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
            }
        }
    }

    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_DELETE)]
    public virtual async Task<IActionResult> Delete(int id)
    {
        //try to get a navbarinfo with the specified id
        var navbar = await _navbarInfoService.GetNavbarInfoByIdAsync(id);
        if (navbar == null)
            return RedirectToAction("List");

        await DeleteStoreMappingsAsync(navbar);
        await _navbarInfoService.DeleteNavbarInfosAsync([navbar]);
        _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Navbars.Deleted"));

        return RedirectToAction("List");
    }

    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_DELETE)]
    public virtual async Task<IActionResult> DeleteSelected(ICollection<int> selectedIds)
    {
        if (selectedIds == null || !selectedIds.Any())
            return NoContent();

        var navbars = await _navbarInfoService.GetNavbarInfoByIdsAsync(selectedIds.ToArray());
        var tasks = new List<Task>();
        foreach (var navbar in navbars)
            tasks.Add(DeleteStoreMappingsAsync(navbar));
        await Task.WhenAll(tasks);

        await _navbarInfoService.DeleteNavbarInfosAsync(navbars);

        return Json(new { Result = true });
    }

    protected virtual async Task DeleteStoreMappingsAsync(NavbarInfo navbarinfo)
    {
        var existingStoreMappings = await _storeMappingService.GetStoreMappingsAsync(navbarinfo);
        var allStores = await _storeService.GetAllStoresAsync();
        foreach (var store in allStores)
        {
            var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
            if (storeMappingToDelete != null)
                await _storeMappingService.DeleteStoreMappingAsync(storeMappingToDelete);
        }
    }
    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_EDIT)]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_DELETE)]
    public virtual async Task<IActionResult> NavbarElementList(NavbarElementSearchModel searchModel)
    {
        var navbar = await _navbarInfoService.GetNavbarInfoByIdAsync(searchModel.NavbarInfoId)
            ?? throw new ArgumentException("No navbar info found with the specified id");

        var model = await _navbarFactory.PrepareNavbarInfoElementListModelAsync(searchModel, navbar);

        return Json(model);
    }

    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_CREATE)]
    public virtual async Task<IActionResult> NavbarElementAddPopup(int categoryId)
    {
        //prepare model
        var model = new NavbarElementModel();
        await _navbarFactory.PrepareNavbarElemenModelAsync(model);

        return View("NavbarElement.CreatePopup.cshtml", model);
    }

    [HttpPost]
    [FormValueRequired("save")]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_CREATE)]

    public virtual async Task<IActionResult> NavbarElementAddPopup(NavbarElementModel model)
    {
        throw new NotImplementedException();
        ////get selected products
        //var selectedProducts = await _productService.GetProductsByIdsAsync(model.SelectedProductIds.ToArray());
        //if (selectedProducts.Any())
        //{
        //    var existingProductCategories = await _categoryService.GetProductCategoriesByCategoryIdAsync(model.CategoryId, showHidden: true);
        //    foreach (var product in selectedProducts)
        //    {
        //        //whether product category with such parameters already exists
        //        if (_categoryService.FindProductCategory(existingProductCategories, product.Id, model.CategoryId) != null)
        //            continue;

        //        //insert the new product category mapping
        //        await _categoryService.InsertProductCategoryAsync(new ProductCategory
        //        {
        //            CategoryId = model.CategoryId,
        //            ProductId = product.Id,
        //            IsFeaturedProduct = false,
        //            DisplayOrder = 1
        //        });
        //    }
        //}

        //ViewBag.RefreshPage = true;

        //return View(new AddProductToCategorySearchModel());
    }
}