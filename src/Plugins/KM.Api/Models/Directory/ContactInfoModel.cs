namespace KM.Api.Models.Directory;

public record ContactInfoModel
{
    public AddressApiModel Address { get; init; }
    public string Comment { get; init; }
    public string Email { get; init; }
    public string Fullname { get; init; }
    public string Phone { get; init; }
    public bool UpdateUserInfo { get; init; }
}
