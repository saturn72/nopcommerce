namespace KM.Api.Services.User;

public static class ExternalUsersServiceExtensions
{
    public static async Task<Customer?> GetCustomerByExternalUserIdAsync(this IExternalUsersService service, string userId, bool provisionIfNotExists = true)
    {
        var map = await service.GetUserIdCustomerMapByExternalUserId(userId);

        if (map == default && provisionIfNotExists)
        {
            var res = await service.ProvisionUsersAsync([userId]);
            return res?.FirstOrDefault()?.Customer;
        }
        return map?.Customer;
    }

}
public interface IExternalUsersService
{
    Task<KmUserCustomerMap> GetUserIdCustomerMapByExternalUserId(string userId);
    Task<IEnumerable<KmUserCustomerMap>> ProvisionUsersAsync(IEnumerable<string> userIds);
}
public record UpdateAddressRequest
{
    public UpdateAddressRequest(Customer customer, Address address)
    {
        Customer = customer;
        Address = address;
    }
    public Customer Customer { get; }
    public Address Address { get; }
}


