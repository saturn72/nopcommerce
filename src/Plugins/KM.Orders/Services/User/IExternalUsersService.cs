namespace KM.Orders.Services.User;

public interface IExternalUsersService
{
    Task<KmUserCustomerMap> GetUserIdCustomerMapByUserId(string userId);
    Task<IEnumerable<KmUserCustomerMap>> ProvisionUsersAsync(IEnumerable<string> userIds);
    Task ProvisionBillingInfosAsync(IEnumerable<UpdateBillingInfoRequest> requests);
    Task ProvisionShippingAddressesAsync(IEnumerable<UpdateShippingAddressRequest> requests);
}
public record UpdateBillingInfoRequest
{
    public UpdateBillingInfoRequest(string userId, Address billingAddress)
    {
        UserId = userId;
        BillingAddress = billingAddress;
    }
    public string UserId { get; }
    public Address BillingAddress { get; }
}

public record UpdateShippingAddressRequest
{
    public UpdateShippingAddressRequest(string userId, Address shippingAddress, Address billingAddress)
    {
        UserId = userId;
        ShippingAddress = shippingAddress;
        BillingAddress = billingAddress;
    }
    public string UserId { get; }
    public Address ShippingAddress { get; }
    public Address BillingAddress { get; }
}


