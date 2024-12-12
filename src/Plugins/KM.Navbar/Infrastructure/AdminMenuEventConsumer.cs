using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace KM.Navbar.Infrastructure;
public class AdminMenuEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    //private readonly IPermissionService _permissionService;

    //public AdminMenuEventConsumer(IPermissionService permissionService)
    //{
    //    _permissionService = permissionService;
    //}

    public async Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        //if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_SETTINGS))
        //    return;

        eventMessage.RootMenuItem.InsertBefore("Promotions",
            new AdminMenuItem
            {
                SystemName = "Navbar",
                Title = "Navbar Management",
                Url = eventMessage.GetMenuItemUrl("NavbarAdmin", "Index"),
                IconClass = "fa-solid fa-bars",
                Visible = true,
            });
    }
}
