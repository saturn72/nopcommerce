using KedemMarket.Domain.User;
using Nop.Core.Domain.Common;

namespace KedemMarket.Services.User;
public interface IExternalUsersService
{
    Task<KmUserCustomerMap> GetUserIdCustomerMapByInternalCustomerId(int customerId);
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


