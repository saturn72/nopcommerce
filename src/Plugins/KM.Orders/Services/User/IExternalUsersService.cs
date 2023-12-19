namespace KM.Orders.Services.User;

public interface IExternalUsersService
{
    Task<KmUserCustomerMap> GetUserIdCustomerMapByUserId(string userId);
    Task<IEnumerable<KmUserCustomerMap>> ProvisionUsersAsync(IEnumerable<string> userIds);
    Task ProvisionBillingInfosAsync(IEnumerable<UpdateBillingInfoRequest> requests);
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