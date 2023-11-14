using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Misc.KM.Orders.Infrastructure
{
    public class SignalRRouteProvider : IRouteProvider
    {
        internal const string Pattern = "/ws/order";
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<OrderHub>(Pattern);
        }

        public int Priority => 0;
    }
}
