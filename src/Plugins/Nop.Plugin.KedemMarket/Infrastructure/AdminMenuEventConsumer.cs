using KedemMarket.Admin.Controllers;
using Nop.Services.Events;
using Nop.Web.Framework.Events;
using Nop.Web.Framework.Menu;

namespace KedemMarket.Infrastructure;
public class AdminMenuEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    //private readonly IPermissionService _permissionService;

    //public AdminMenuEventConsumer(IPermissionService permissionService)
    //{
    //    _permissionService = permissionService;
    //}

    public Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
        //if (!await _permissionService.AuthorizeAsync(StandardPermission.Configuration.MANAGE_SETTINGS))
        //    return;

        eventMessage.RootMenuItem.InsertBefore("Promotions",
            new AdminMenuItem
            {
                SystemName = "Navbar",
                Title = "Navbar",
                Url = eventMessage.GetMenuItemUrl("Navbar", nameof(NavbarController.Index)),
                IconClass = "fa-solid fa-bars",
                Visible = true,
            });
        return Task.CompletedTask;
    }
}
