namespace KM.Api.Models.Checkout;

public record ContactInfoModel
{
    public AddressApiModel Address { get; init; }
    public string Comment { get; init; }
    public string Email { get; init; }
    public string Fullname { get; init; }
    public string Phone { get; init; }
    public bool UpdateUserInfo { get; init; }
}

public record AddressApiModel
{
    public string City { get; init; }
    public string PostalCode { get; init; }
    public string Street { get; init; }
}
