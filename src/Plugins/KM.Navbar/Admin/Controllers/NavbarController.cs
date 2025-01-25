using KM.Navbar.Admin.Models;
using Nop.Web.Framework.Mvc;

namespace KM.Navbar.Admin.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin] //confirms access to the admin panel
[Area(AreaNames.ADMIN)] //specifies the area containing a controller or action
public class NavbarController : BaseAdminController
{
    private const string VIEW_PATH = "~/Plugins/KM.Navbar/Admin/Views/";
    private readonly INavbarFactory _navbarFactory;
    private readonly INavbarService _navbarInfoService;
    private readonly INotificationService _notificationService;
    private readonly ILocalizationService _localizationService;
    private readonly IStoreMappingService _storeMappingService;
    private readonly IStoreService _storeService;

    public NavbarController(
        INavbarFactory navbarFactory,
        INavbarService navbarInfoService,
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
            await _navbarInfoService.InsertNavbarInfoAsync(navbar);

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

    #region Elements    
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
    public virtual async Task<IActionResult> NavbarElementCreate(int navbarInfoId)
    {
        var model = new CreateOrUpdateNavbarElementModel
        {
            NavbarInfoId = navbarInfoId
        };
        await _navbarFactory.PrepareCreateOrUpdateNavbarElementModelAsync(model);

        return View("NavbarElement/Create.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_CREATE)]
    public virtual async Task<IActionResult> NavbarElementCreate(CreateOrUpdateNavbarElementModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var navbarElement = model.ToEntity<NavbarElement>();
            await _navbarInfoService.InsertNavbarElementAsync(navbarElement);
            var msg = await _localizationService.GetResourceAsync("Admin.Navbars.Elements.Added");
            _notificationService.SuccessNotification(msg);
            if (continueEditing)
                return RedirectToAction(nameof(EditNavbarElement), new { id = model.NavbarInfoId });
            return RedirectToAction("Edit", new { id = model.NavbarInfoId });
        }

        await _navbarFactory.PrepareCreateOrUpdateNavbarElementModelAsync(model);
        return View("NavbarElement/Create.cshtml", model);
    }

    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_EDIT)]
    public virtual async Task<IActionResult> EditNavbarElement(int id)
    {
        var e = await _navbarInfoService.GetNavbarElementsByIdAsync(id);
        var model = e.ToModel<CreateOrUpdateNavbarElementModel>();
        model.NavbarElementId = id;
        await _navbarFactory.PrepareCreateOrUpdateNavbarElementModelAsync(model);
        return View("NavbarElement/Edit.cshtml", model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_EDIT)]
    public virtual async Task<IActionResult> EditNavbarElement(CreateOrUpdateNavbarElementModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var navbarElement = model.ToEntity<NavbarElement>();
            await _navbarInfoService.UpdateNavbarElementAsync(navbarElement);
            var msg = await _localizationService.GetResourceAsync("Admin.Navbars.Elements.Updated");
            _notificationService.SuccessNotification(msg);
        }
        if (continueEditing)
            return RedirectToAction(nameof(EditNavbarElement), new { id = model.Id });
        return RedirectToAction("Edit", new { id = model.NavbarInfoId });
    }

    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_DELETE)]
    public virtual async Task<IActionResult> DeleteNavbarElement(NavbarElementModel model)
    {
        if (ModelState.IsValid)
        {
            var navbarElement = model.ToEntity<NavbarElement>();
            await _navbarInfoService.DeleteNavbarElementAsync(navbarElement);
            var msg = await _localizationService.GetResourceAsync("Admin.Navbars.Elements.Deleted");
            _notificationService.SuccessNotification(msg);
        }
        return new NullJsonResult();
    }

    #endregion

    #region Vendors
    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_VIEW)]
    public virtual async Task<IActionResult> NavbarElementVendorList(NavbarElementVendorListSearchModel searchModel)
    {
        var model = await _navbarFactory.PrepareNavbarElementVendorListSearchModelAsync(searchModel);
        return Json(model);
    }

    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_EDIT)]
    public virtual async Task<IActionResult> VendorAddPopup(int navbarElementId)
    {
        var searchModel = new AddVendorToNavbarElementSearchModel
        {
            NavbarElementId = navbarElementId
        };
        await _navbarFactory.PrepareAddVendorToNavbarElementModel(searchModel);

        return View("NavbarElement/VendorAddPopup.cshtml", searchModel);
    }
    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_VIEW)]
    public virtual async Task<IActionResult> VendorAddPopupList(NavbarElementVendorListSearchModel searchModel)
    {
        var model = await _navbarFactory.VendorAddPopupListAsync(searchModel);
        return Json(model);
    }

    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_EDIT)]
    public virtual async Task<IActionResult> VendorAddPopup(AddVendorToNavbarElementSearchModel model)
    {
        await _navbarInfoService.AddNavbarElementVendorsAsync(model.NavbarElementId, model.SelectedVendorIds);
        ViewBag.RefreshPage = true;
        var msg = await _localizationService.GetResourceAsync("Admin.Navbars.Elements.Vendors.Deleted");
        _notificationService.SuccessNotification(msg);
        return View("NavbarElement/VendorAddPopup.cshtml", new AddVendorToNavbarElementSearchModel());
    }

    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_DELETE)]
    public virtual async Task<IActionResult> DeleteNavbarElementVendor(int id)
    {
        if (id <= 0)
            return new NullJsonResult();

        var nev = await _navbarInfoService.GetNavbarElementVendorByIdAsync(id);
        await _navbarInfoService.DeleteNavbarElementVendorAsync(nev);
        var msg = await _localizationService.GetResourceAsync("Admin.Navbars.Elements.Vendors.Added");
        _notificationService.SuccessNotification(msg);
        ViewBag.RefreshPage = true;
        return new NullJsonResult();
    }


    [CheckPermission(NavbarPermissions.NAVBARS_ELEMENTS_EDIT)]
    public async Task<IActionResult> UpdateNavbarElementVendor(NavbarElementVendorModel model)
    {
        var nev = await _navbarInfoService.GetNavbarElementVendorByIdAsync(model.Id)
            ?? throw new ArgumentException("No navbar element to vendor mapping found with the specified id");

        nev.DisplayOrder = model.DisplayOrder;
        nev.IsFeaturedVendor = model.IsFeaturedVendor;
        nev.Published = model.Published;
        await _navbarInfoService.UpdateNavbarElementVendorAsync(nev);

        return new NullJsonResult();
    }
    #endregion
}