using KM.Api.Models.Directory;
using KM.Api.Services.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Attributes;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Nop.Web.Areas.Admin.Models.Common;

namespace KM.Api.Controllers;

[Route("api/store")]
public class StoreController : KmApiControllerBase
{
    private readonly IStoreContext _storeContext;
    private readonly IVendorService _vendorService;
    private readonly IAddressService _addressService;
    private readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    private readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;
    private readonly IPictureService _pictureService;
    private readonly MediaPreperar _mediaPreperar;

    public StoreController(
        IStoreContext storeContext,
        IVendorService vendorService,
        IAddressService addressService,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService,
        IPictureService pictureService,
        MediaPreperar mediaPreperar)
    {
        _storeContext = storeContext;
        _vendorService = vendorService;
        _addressService = addressService;
        _addressAttributeParser = addressAttributeParser;
        _addressAttributeService = addressAttributeService;
        _pictureService = pictureService;
        _mediaPreperar = mediaPreperar;
    }

    [HttpGet]
    public async Task<IActionResult> GetStoreInfoByStoreIdAsync()
    {
        var store = await _storeContext.GetCurrentStoreAsync();
        if (store == default)
            return BadRequest();

        var phone = ProcessPhoneNumber(store.CompanyPhoneNumber);
        var data = new
        {
            storeName = store.Name,
            displayName = store.DefaultTitle,
            phone,
            url = store.Url,
            socialLinks = new
            {
                facebook = "https://www.facebook.com/KedemMarket.co.il",
                instagram = "https://www.instagram.com/kedemmarket.co.il/",
                linktr = "https://linktr.ee/kedemmarket",
            }
        };
        return Ok(data);
    }
    private string ProcessPhoneNumber(string? sourcePhone)
    {
        if (sourcePhone == null)
            return null;
        //remove non number characters
        var res = new List<char>();
        for (var i = 0; i < sourcePhone.Length; i++)
        {
            var nv = char.GetNumericValue(sourcePhone[i]);
            if (nv < 0 || nv > 9)
                continue;
            res.Add(sourcePhone[i]);
        }
        if (res.ElementAt(0) != 0)
            res.Insert(0, '+');

        return new string(res.ToArray());
    }

    [HttpGet("vendor")]
    public async Task<IActionResult> GetVendorsAsync([FromQuery] int pageSize = 25, [FromQuery] int offset = 0)
    {
        var v1 = await _vendorService.GetAllVendorsAsync(pageIndex: offset / pageSize, pageSize: pageSize);
        var vvs = v1
            .Where(v => v.Active && !v.Deleted)
            .ToList();

        var vendors = new List<object>();
        foreach (var v in vvs)
            vendors.Add(await ToVendorApiModel(v));

        return Ok(new { vendors });
    }
    private async Task<object> ToVendorApiModel(Vendor vendor)
    {
        var address = await _addressService.GetAddressByIdAsync(vendor.AddressId);
        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);

        return new
        {
            id = vendor.Id,
            name = vendor.Name,
            description = vendor.Description,
            contactInfo = await ToContactInfo(address),
            displayOrder = vendor.DisplayOrder,
            metaKeywords = vendor.MetaKeywords,
            metaDescription = vendor.MetaDescription,
            metaTitle = vendor.MetaTitle,
            image = _mediaPreperar.ToMediaItemAsync(picture)
        };
        //PictureId,
    }

    private async Task<ContactInfoModel?> ToContactInfo(Address address)
    {
        if (address == default)
            return null;

        var model = new AddressModel();
        var attribute = (await _addressAttributeService.GetAllAttributesAsync())
            .FirstOrDefault(a => a.Name.Equals("comment", StringComparison.CurrentCultureIgnoreCase));

        var comment = default(string);

        if (attribute != default && address.CustomAttributes != default)
        {
            var enteredText = _addressAttributeParser.ParseValues(address.CustomAttributes, attribute.Id);
            if (enteredText.Any())
                comment = enteredText[0];
        }

        return new()
        {
            Address = new AddressApiModel
            {
                City = address.City,
                PostalCode = address.ZipPostalCode,
                Street = buildTrimedString(address.Address1, address.Address2),
            },
            Comment = comment,
            Email = address.Email,
            Fullname = buildTrimedString(address.FirstName, address.LastName),
            Phone = ProcessPhoneNumber(address.PhoneNumber),
        };

        static string buildTrimedString(string? str1, string? str2) =>
            $"{str1 ?? ""} {str2 ?? ""}".Trim();
    }
}
