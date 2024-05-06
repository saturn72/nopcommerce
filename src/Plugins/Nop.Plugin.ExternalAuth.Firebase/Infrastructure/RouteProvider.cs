using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.ExternalAuth.Firebase.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapControllerRoute(FirebaseAuthenticationDefaults.DataDeletionCallbackRoute, $"Firebase/data-deletion-callback/",
                new { controller = "FirebaseDataDeletion", action = "DataDeletionCallback" });

            endpointRouteBuilder.MapControllerRoute(FirebaseAuthenticationDefaults.DataDeletionStatusCheckRoute, $"Firebase/data-deletion-status-check/{{earId:min(0)}}",
                new { controller = "FirebaseAuthentication", action = "DataDeletionStatusCheck" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
