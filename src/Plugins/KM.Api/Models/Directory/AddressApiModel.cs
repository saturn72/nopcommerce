namespace KM.Api.Models.Directory;

public record AddressApiModel
{
    public string City { get; init; }
    public string PostalCode { get; init; }
    public string Street { get; init; }
}
