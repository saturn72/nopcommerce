using Microsoft.AspNetCore.Mvc.Razor;

namespace Nop.Plugin.Misc.KM.Orders.Infrastructure
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            if (context.AreaName == "Admin")
            {
                viewLocations = new[] { $"/Plugins/Nop.Plugin.Misc.KM.Orders/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
            }
            else
            {
                viewLocations = new[] { $"/Plugins/Nop.Plugin.Misc.KM.Orders/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
            }

            return viewLocations;
        }
    }
}
