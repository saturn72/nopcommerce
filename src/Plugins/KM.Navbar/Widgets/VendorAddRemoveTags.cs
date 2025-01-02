using Nop.Web.Framework.Components;

namespace KM.Navbar.Widgets;
public  class VendorAddRemoveTags : NopViewComponent
{
    public IViewComponentResult Invoke(string widgetZone, object additionalData)
    {
        return Content("Hello World from VendorAddRemoveTags");
    }
}