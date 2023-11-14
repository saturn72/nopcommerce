using Nop.Plugin.Misc.KM.Orders.Models;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.KM.Orders.Factories
{
    public interface IKMUserCartModelFactory
    {
        Task PrepareUserCartModelAsync(UserCartModel model);
    }
}
