using KM.Api.Models.Directory;
using KM.Api.Models.User;
using Nop.Services.Attributes;
using Nop.Services.Media;

namespace KM.Api.Factories;

public class VendorApiModelFactory : IVendorApiModelFactory
{
    private readonly IPictureService _pictureService;
    private readonly IAddressService _addressService;
    private readonly MediaConvertor _mediaPreperar;
    private readonly IAttributeParser<AddressAttribute, AddressAttributeValue> _addressAttributeParser;
    private readonly IAttributeService<AddressAttribute, AddressAttributeValue> _addressAttributeService;
    private readonly IDirectoryFactory _directoryFactory;
    public VendorApiModelFactory(
        IPictureService pictureService,
        IAddressService addressService,
        MediaConvertor mediaPreperar,
        IAttributeParser<AddressAttribute, AddressAttributeValue> addressAttributeParser,
        IAttributeService<AddressAttribute, AddressAttributeValue> addressAttributeService
,
        IDirectoryFactory directoryFactory)
    {
        _pictureService = pictureService;
        _addressService = addressService;
        _mediaPreperar = mediaPreperar;
        _addressAttributeParser = addressAttributeParser;
        _addressAttributeService = addressAttributeService;
        _directoryFactory = directoryFactory;
    }
    public async Task<VendorApiModel> ToVendorApiModel(Vendor vendor)
    {
        var address = await _addressService.GetAddressByIdAsync(vendor.AddressId);
        var picture = await _pictureService.GetPictureByIdAsync(vendor.PictureId);
        var image = picture != default ? await _mediaPreperar.ToGalleryItemModel(picture, 0) : default;

        var contactInfo = await ToContactInfo(vendor, address);
        return new()
        {
            Id = vendor.Id,
            Name = vendor.Name,
            Description = vendor.Description,
            ContactInfo = contactInfo,
            DisplayOrder = vendor.DisplayOrder,
            MetaKeywords = vendor.MetaKeywords,
            MetaDescription = vendor.MetaDescription,
            MetaTitle = vendor.MetaTitle,
            Image = image,
        };
        //PictureId,
    }

    private async Task<ContactInfoModel?> ToContactInfo(Vendor vendor, Address? address)
    {
        var attribute = (await _addressAttributeService.GetAllAttributesAsync())
            .FirstOrDefault(a => a.Name.Equals("comment", StringComparison.CurrentCultureIgnoreCase));

        var comment = default(string);

        if (address != null && attribute != default && address.CustomAttributes != default)
        {
            var enteredText = _addressAttributeParser.ParseValues(address?.CustomAttributes, attribute.Id);
            if (enteredText.Any())
                comment = enteredText[0];
        }

        var street = buildTrimedString(address?.Address1, address?.Address2);
        if (street.HasNoValue())
            street = null;

        return new()
        {
            Address = new AddressApiModel
            {
                City = address?.City,
                PostalCode = address?.ZipPostalCode,
                Street = street,
            },
            Comment = comment,
            Email = address?.Email ?? vendor.Email,
            Fullname = buildTrimedString(address?.FirstName ?? vendor.Name, address?.LastName),
            Phone = _directoryFactory.ProcessPhoneNumber(address?.PhoneNumber),
        };

        static string buildTrimedString(string? str1, string? str2) =>
            $"{str1 ?? ""} {str2 ?? ""}".Trim();
    }
}
