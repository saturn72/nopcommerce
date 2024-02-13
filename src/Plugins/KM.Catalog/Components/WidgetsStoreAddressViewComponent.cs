using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace KM.Catalog.Components
{
    public class WidgetsStoreAddressViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            return View("~/Plugins/KM.Catalog/Views/StoreAddress.cshtml", additionalData);
        }
    }
}
