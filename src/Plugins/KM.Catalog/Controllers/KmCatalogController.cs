using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc.Filters;
using KM.Catalog.Models;
using KM.Catalog.Factories;
using Nop.Web.Framework.Controllers;

namespace KM.Catalog.Controllers;

[AutoValidateAntiforgeryToken]
[AuthorizeAdmin]
[Area(AreaNames.Admin)]
public class KmCatalogController : BasePluginController
{
    private readonly IKmCatalogModelFactory _modelFactory;
    private readonly KmCatalogSettings _kmCatalogSettings;

    public KmCatalogController(
        IKmCatalogModelFactory modelFactory,
        KmCatalogSettings kmCatalogSettings)
    {
        _modelFactory = modelFactory;
        _kmCatalogSettings = kmCatalogSettings;
    }
    public async Task<IActionResult> Configure()
    {
        var model = new ConfigurationModel();
        await _modelFactory.PrepareConfigurationModelAsync(model);

        return View("~/Plugins/KM.Catalog/Views/Configure.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Configure(ConfigurationModel model)
    {
        if (!ModelState.IsValid)
            return await Configure();

        _kmCatalogSettings.DefaultVendorId = model.VendorId;
        return await Configure();
    }
}