using KM.Api.Middlewares;
using KM.Common.Services.Media;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KM.Api.Infrastructure;

public class PluginNopStartup : INopStartup
{
    private const string CorsPolicy = "api-order-cors";
    private const string CatalogWSCorsPolicy = "catalog-ws-cors";

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

            options.AddPolicy(name: CatalogWSCorsPolicy,
                          policy => policy.WithOrigins(origins)
                              .WithMethods("GET", "POST")
                              .AllowCredentials()
                              .AllowAnyHeader());
        });

        services.AddMemoryCache();
        services.TryAddSingleton<MediaConvertor>();

        services.AddScoped<IExternalUsersService, FirebaseExternalUsersService>();
        services.AddTransient<IValidator<CartTransactionApiModel>, CartTransactionApiModelValidator>();
        services.AddScoped<IKmOrderService, KmOrderService>();

        services.AddScoped<IOrderDocumentStore, OrderDocumentStore>();
        services.AddScoped<IUserProfileDocumentStore, UserProfileDocumentStore>();
        services.AddScoped(typeof(IDocumentStore<>), typeof(FirebaseDocumentStore<>));
        services.AddSingleton<FirebaseAdapter>();
        services.AddScoped<IProductApiFactory, ProductApiFactory>();
        services.AddScoped<IVendorApiModelFactory, VendorApiModelFactory>();
        services.AddScoped<IShoppingCartFactory, ShoppingCartFactory>();
        services.AddScoped<IOrderApiModelFactory, OrderApiModelFactory>();
        services.AddScoped<IDirectoryFactory, DirectoryFactory>();

        services.AddSignalR();
        services.TryAddScoped<IStorageManager, GcpStorageManager>();

        services.Configure<GcpOptions>(options =>
        {
            var bn = configuration["gcpOptions:bucketName"];
            if (string.IsNullOrEmpty(bn) || string.IsNullOrWhiteSpace(bn))
                throw new ArgumentException(nameof(GcpOptions.BucketName));
            options.BucketName = bn;
        });

        services.AddSingleton<IPriorityQueue, PriorityQueue>();
    }

    public void Configure(IApplicationBuilder application)
    {
        application.UseCors(CatalogWSCorsPolicy);
        application.UseCors(CorsPolicy);
        application.UseWhen(
            ctx => ctx.Request.Path.StartsWithSegments("/api"),
            appBuilder => appBuilder.UseMiddleware<KmAuthenticationMiddleware>());
    }


    public int Order => 10;
}