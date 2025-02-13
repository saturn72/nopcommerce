using Microsoft.AspNetCore.Http;

namespace KedemMarket.Api.Middlewares;
public class KedemMarketAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public KedemMarketAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var eus = context.RequestServices.GetService<IExternalUsersService>();
        if (context.Request.Headers.TryGetValue(KmApiConsts.USER_ID, out var userId) &&
            !string.IsNullOrEmpty(userId) &&
            !string.IsNullOrWhiteSpace(userId))
        {
            var customer = await eus.GetCustomerByExternalUserIdAsync(userId, false);
            var wc = context.RequestServices.GetService<IWorkContext>();
            await wc.SetCurrentCustomerAsync(customer);
        }

        await _next(context);
    }
}