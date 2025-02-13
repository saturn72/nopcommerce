using KedemMarket.Admin.Controllers;
using Nop.Web.Framework.Menu;

namespace KedemMarket.Infrastructure;
public class AdminMenuEventConsumer : IConsumer<AdminMenuCreatedEvent>
{
    public Task HandleEventAsync(AdminMenuCreatedEvent eventMessage)
    {
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
