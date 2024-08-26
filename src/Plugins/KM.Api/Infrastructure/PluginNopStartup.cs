
using KM.Api.Controllers;
using KM.Api.Factories;
using KM.Api.Middlewares;

namespace KM.Api.Infrastructure;

public class PluginNopStartup : INopStartup
{
    private const string CorsPolicy = "api-order-cors";
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        var origins = configuration.GetRequiredSection("cors:origins")
            .AsEnumerable()
            .Where(o => o.Value != default && new Uri(o.Value) != null)
            .Select(o => o.Value)
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicy,
                policy => policy.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod());
        });

        services.AddMemoryCache();
        services.AddSingleton<MediaPreperar>();

        services.AddScoped<IExternalUsersService, FirebaseExternalUsersService>();
        services.AddTransient<IValidator<CartTransactionApiModel>, CartTransactionApiModelValidator>();
        services.AddScoped<IKmOrderService, KmOrderService>();

        services.AddScoped<IOrderDocumentStore, OrderDocumentStore>();
        services.AddScoped<IUserProfileDocumentStore, UserProfileDocumentStore>();
        services.AddScoped(typeof(IDocumentStore<>), typeof(FirebaseDocumentStore<>));
        services.AddSingleton<FirebaseAdapter>();
        services.AddScoped<IProductApiFactory, ProductApiFactory>();
        services.AddScoped<ICheckoutCartApiFactory, CheckoutCartApiFactory>();
        services.AddSignalR();
    }

    public void Configure(IApplicationBuilder application)
    {
        application.UseCors(CorsPolicy);
        application.UseWhen(
            ctx => ctx.Request.Path.StartsWithSegments("/api"),
            appBuilder => appBuilder.UseMiddleware<KmAuthenticationMiddleware>());

    }


    public int Order => 10;
}