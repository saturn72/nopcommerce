
using Nop.Core.Domain.Discounts;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;

namespace KM.Navbar.Admin.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin] //confirms access to the admin panel
[Area(AreaNames.ADMIN)] //specifies the area containing a controller or action
public class NavbarController : BaseAdminController
{
    private const string VIEW_PATH = "~/Plugins/KM.Navbar/Admin/Views/";
    private readonly INavbarFactory _navbarFactory;

    public NavbarController(INavbarFactory navbarFactory)
    {
        _navbarFactory = navbarFactory;
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
        return View(VIEW_PATH + "List.cshtml", model);
    }

    [HttpPost]
    [CheckPermission(NavbarPermissions.NAVBARS_VIEW)]
    public virtual async Task<IActionResult> List(NavbarInfoSearchModel searchModel)
    {
        //prepare model
        var model = await _navbarFactory.PrepareNavbarInfoListModelAsync(searchModel);

        return Json(model);
    }



    [CheckPermission(NavbarPermissions.NAVBARS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create()
    {
        //prepare model
        var model = await _navbarFactory.PrepareNavbarInfoModelAsync(new NavbarInfoModel(), null);

        return View(model);
    }

    [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
    [CheckPermission(NavbarPermissions.NAVBARS_CREATE_EDIT_DELETE)]
    public virtual async Task<IActionResult> Create(NavbarInfoModel model, bool continueEditing)
    {
        if (ModelState.IsValid)
        {
            var navbar = model.ToEntity<Navbar>();
            navbar.CreatedOnUtc = DateTime.UtcNow;
            navbar.UpdatedOnUtc = DateTime.UtcNow;
            await _navbarService.InsertNavbarAsync(navbar);

            //search engine name
            model.SeName = await _urlRecordService.ValidateSeNameAsync(navbar, model.SeName, navbar.Name, true);
            await _urlRecordService.SaveSlugAsync(navbar, model.SeName, 0);

            //locales
            await UpdateLocalesAsync(navbar, model);

            //discounts
            var allDiscounts = await _discountService.GetAllDiscountsAsync(DiscountType.AssignedToNavbars, showHidden: true, isActive: null);
            foreach (var discount in allDiscounts)
            {
                if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    await _navbarService.InsertDiscountNavbarMappingAsync(new DiscountNavbarMapping { DiscountId = discount.Id, EntityId = navbar.Id });
            }

            await _navbarService.UpdateNavbarAsync(navbar);

            //update picture seo file name
            await UpdatePictureSeoNamesAsync(navbar);

            //stores
            await SaveStoreMappingsAsync(navbar, model);

            //activity log
            await _customerActivityService.InsertActivityAsync("AddNewNavbar",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewNavbar"), navbar.Name), navbar);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Catalog.Navbars.Added"));

            if (!continueEditing)
                return RedirectToAction("List");

            return RedirectToAction("Edit", new { id = navbar.Id });
        }

        //prepare model
        model = await _navbarModelFactory.PrepareNavbarModelAsync(model, null, true);

        //if we got this far, something failed, redisplay form
        return View(model);
    }
}