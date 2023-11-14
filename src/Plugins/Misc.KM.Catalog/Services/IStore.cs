using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.KM.Catalog.Services
{
    public interface IStore<TDocument>
    {
        Task CreateOrUpdateAsync(IEnumerable<TDocument> documents);
    }
}
