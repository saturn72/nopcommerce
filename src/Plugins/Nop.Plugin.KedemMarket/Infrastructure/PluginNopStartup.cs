

using KedemMarket.Admin.Models.Navbar;
using KedemMarket.Middlewares;
using KedemMarket.Services.Orders;

namespace KedemMarket.Infrastructure;

public class PluginNopStartup : INopStartup
{
    private const string CorsPolicy = "kedemmarket-api-cors";
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

        services.AddScoped<IExternalUsersService, FirebaseExternalUsersService>();
        services.AddTransient<IValidator<CartTransactionApiModel>, CartTransactionApiModelValidator>();
        services.AddScoped<IKmOrderService, KmOrderService>();

        services.AddScoped<IOrderDocumentStore, OrderDocumentStore>();
        services.AddScoped<IUserProfileDocumentStore, UserProfileDocumentStore>();
        services.AddScoped(typeof(IDocumentStore<>), typeof(FirebaseDocumentStore<>));
        services.AddSingleton<FirebaseAdapter>();
        services.AddScoped<IVendorApiModelFactory, VendorApiModelFactory>();
        services.AddScoped<IShoppingCartFactory, ShoppingCartFactory>();
        services.AddScoped<IOrderApiModelFactory, OrderApiModelFactory>();
        services.AddScoped<IDirectoryFactory, DirectoryFactory>();


        //new KM.Common.Infrastructure.NopStartup().ConfigureServices(services, configuration);
        services.AddScoped<INavbarService, NavbarService>();
        services.AddScoped<INavbarFactory, NavbarFactory>();
        services.AddScoped<KedemMarket.Factories.Navbar.INavbarFactory, KedemMarket.Factories.Navbar.NavbarFactory>();
        services.AddScoped<INavbarFactory, NavbarFactory>();
        services.AddTransient<IValidator<NavbarInfoModel>, NavInfoModelValidator>();
        services.AddTransient<IValidator<CreateOrUpdateNavbarElementModel>, CreateNavbarElementPopupModelValidator>();

        services.TryAddScoped<IProductApiFactory, ProductApiFactory>();
        services.TryAddSingleton<MediaConvertor>();
        services.TryAddSingleton(TimeProvider.System);

        services.TryAddScoped<IStorageManager, GcpStorageManager>();
        //do not configure if already configured
        if (services.FirstOrDefault(x => x.ServiceType == typeof(GcpOptions)) != null)
            return;

        services.Configure<GcpOptions>(options =>
        {
            var bn = configuration["gcpOptions:bucketName"];
            if (string.IsNullOrEmpty(bn) || string.IsNullOrWhiteSpace(bn))
                throw new ArgumentException(nameof(GcpOptions.BucketName));
            options.BucketName = bn;
        });
    }

    public void Configure(IApplicationBuilder application)
    {
        application.UseCors(CorsPolicy);
        application.UseWhen(
            ctx => ctx.Request.Path.StartsWithSegments("/api"),
            appBuilder => appBuilder.UseMiddleware<KedemMarketAuthenticationMiddleware>());
    }

    public int Order => 10;
}