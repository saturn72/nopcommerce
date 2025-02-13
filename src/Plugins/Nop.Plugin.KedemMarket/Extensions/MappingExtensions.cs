namespace KedemMarket.Extensions;

internal static class MappingExtensions
{
    internal static Address ToAddress(this ContactInfoModel ci)
    {
        var names = ci.Fullname.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lastName = ci.Fullname[names[0].Length..].Trim();
        return new Address
        {
            FirstName = names[0],
            LastName = lastName,
            Email = ci.Email,
            PhoneNumber = ci.Phone,
            Address1 = ci.Address.Street,
            City = ci.Address.City,
            ZipPostalCode = ci.Address.PostalCode,
            //CountryId == need to add countryId
        };
    }

    public static AddressApiModel ToAddressApiModel(this AddressModel address)
    {
        return new AddressApiModel
        {
            City = address.City,
            PostalCode = address.ZipPostalCode,
            Street = MergeBothIfSecondNotEmpty(address.Address1, address.Address2),
        };
    }
    public static ContactInfoModel ToContactInfoModel(this AddressModel address)
    {
        return new ContactInfoModel
        {
            Address = address.ToAddressApiModel(),
            Email = address.Email,
            Fullname = MergeBothIfSecondNotEmpty(address.FirstName, address.LastName),
            Phone = address.PhoneNumber,
        };

    }
    static string MergeBothIfSecondNotEmpty(string s1, string s2)
    {
        return s2.IsNullOrEmpty() ? s1 : s1 + " " + s2;
    }
}