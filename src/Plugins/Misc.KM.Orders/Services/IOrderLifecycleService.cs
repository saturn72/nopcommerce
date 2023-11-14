
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.KM.Orders.Services
{
    public interface IOrderLifecycleService
    {
        Task CancelOrderAsync(string orderId, params string[] cancellationReasons);
    }
}
