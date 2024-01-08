using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace KM.Catalog.Models;

public record ConfigurationModel : BaseNopModel
{
    [NopResourceDisplayName("KM.Catalog.DefaultVendor")]
    public int VendorId { get; set; }
    public IList<SelectListItem> Vendors { get; set; }

}
