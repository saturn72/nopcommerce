
namespace Km.Orders.Infrastructure
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
