namespace Nop.Plugin.Misc.KM.Orders.Services.User;

public interface IExternalUsersService
{
    Task<KmUserCustomerMap> GetUserIdCustomerMapByUserId(string userId);
    Task<IEnumerable<KmUserCustomerMap>> ProvisionUsersAsync(IEnumerable<string> userIds);
}
