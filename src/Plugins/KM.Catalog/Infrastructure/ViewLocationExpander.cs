﻿namespace Km.Catalog.Infrastructure;

public class ViewLocationExpander : IViewLocationExpander
{
    public void PopulateValues(ViewLocationExpanderContext context)
    {
    }

    public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
    {
        if (context.AreaName == "Admin")
        {
            viewLocations = new[] { $"/Plugins/Km.Catalog/Areas/Admin/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }
        else
        {
            viewLocations = new[] { $"/Plugins/Km.Catalog/Views/{context.ControllerName}/{context.ViewName}.cshtml" }.Concat(viewLocations);
        }

        return viewLocations;
    }
}
