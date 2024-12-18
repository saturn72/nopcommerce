
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

    public NavbarController(
        INavbarFactory navbarFactory,
        INavbarInfoService navbarInfoService,
        INotificationService notificationService,
        ILocalizationService localizationService)
    {
        _navbarFactory = navbarFactory;
        _navbarInfoService = navbarInfoService;
        _notificationService = notificationService;
        _localizationService = localizationService;
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

            var msg = await _localizationService.GetResourceAsync("Admin.Catalog.Navbars.Added");
            _notificationService.SuccessNotification(msg);

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = navbar.Id });
        }

        model = await _navbarFactory.PrepareNavbarInfoModelAsync(model, null, true);
        return View("Create.cshtml", model);
    }
}