
namespace KM.Orders.Consumer;

public class CustomerUpdateConsumer : IConsumer<EntityUpdatedEvent<Customer>>
{
    private readonly IRepository<KmUserCustomerMap> _userCustomerMapRepository;

    public CustomerUpdateConsumer(
        IRepository<KmUserCustomerMap> userCustomerMapRepository)
    {
        _userCustomerMapRepository = userCustomerMapRepository;
    }
    public async Task HandleEventAsync(EntityUpdatedEvent<Customer> eventMessage)
    {
        var map = _userCustomerMapRepository.Table.FirstOrDefault(c => c.CustomerId == eventMessage.Entity.Id);
        if (map == default)
            return;

        map.ShouldProvisionBasicClaims = false;
        await _userCustomerMapRepository.UpdateAsync(map, false);
    }
}
