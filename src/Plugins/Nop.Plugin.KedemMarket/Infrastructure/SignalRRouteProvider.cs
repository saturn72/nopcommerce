﻿using Nop.Web.Framework.Mvc.Routing;

namespace KedemMarket.Infrastructure;

public class SignalRRouteProvider : IRouteProvider
{
    public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
    {
        //endpointRouteBuilder.MapHub<OrderHub>("/ws/order");
        //endpointRouteBuilder.MapHub<CatalogHub>("/ws/catalog");
    }

    public int Priority => 0;
}
