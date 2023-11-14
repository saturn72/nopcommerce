
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.KM.Orders.Services
{
    public class OrderLifecycleService : IOrderLifecycleService
    {
        public Task CancelOrderAsync(string orderId, params string[] cancellationReasons)
        {
            throw new System.NotImplementedException("insert to user's orders sub-collection");
        }
    }
}
