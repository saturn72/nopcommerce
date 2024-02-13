namespace Km.Orders.Services.User;

public interface IExternalUsersService
{
    Task<KmUserCustomerMap> GetUserIdCustomerMapByUserId(string userId);
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


