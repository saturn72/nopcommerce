namespace KedemMarket.Services.User;

public static class ExternalUsersServiceExtensions
{
    public static async Task<Customer> GetCustomerByExternalUserIdAsync(this IExternalUsersService service, string userId, bool provisionIfNotExists = true)
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


