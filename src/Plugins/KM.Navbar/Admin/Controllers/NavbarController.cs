using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework;
using Nop.Web.Areas.Admin.Controllers;

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

    //[CheckPermission(Nop.Services.Security.StandardPermission.)]
    public virtual async Task<IActionResult> List()
    {
        var model = await _navbarFactory.PrepareNavbarListAsync();
        return View(VIEW_PATH + "List.cshtml", model);
    }
}