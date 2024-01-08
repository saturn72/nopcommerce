using KM.Catalog.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KM.Catalog.Factories;
public class KmCatalogModelFactory : IKmCatalogModelFactory
{
    private readonly IVendorService _vendorService;

    private readonly KmCatalogSettings _kmCatalogSettings;

    public KmCatalogModelFactory(
        IVendorService vendorService,
        KmCatalogSettings kmCatalogSettings)
    {
        _vendorService = vendorService;
        _kmCatalogSettings = kmCatalogSettings;
    }
    public async Task PrepareConfigurationModelAsync(ConfigurationModel model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        model.Vendors = new List<SelectListItem>();

        //prepare available product templates
        var vendors = (await _vendorService.GetAllVendorsAsync()).OrderBy(v => v.Name);


        foreach (var vendor in vendors)
        {
            var item = new SelectListItem
            {
                Value = vendor.Id.ToString(),
                Text = vendor.Name,
            };

            if (item.Selected)
            {
                model.Vendors.Insert(0, item);
            }
            model.Vendors.Add(item);
        }
        if (vendors.Count() == 1)
            model.Vendors.RemoveAt(1);
    }
}
