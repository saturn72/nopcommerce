namespace KedemMarket.Middlewares;
public class KedemMarketAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public KedemMarketAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(KmConsts.USER_ID, out var userId) &&
            !string.IsNullOrEmpty(userId) &&
            !string.IsNullOrWhiteSpace(userId))
        {
            var eus = context.RequestServices.GetService<IExternalUsersService>();
            var customer = await eus.GetCustomerByExternalUserIdAsync(userId);
            var wc = context.RequestServices.GetService<IWorkContext>();
            await wc.SetCurrentCustomerAsync(customer);
        }

        await _next(context);
    }
}