
namespace KM.Orders.Infrastructure
{
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
            services.AddScoped<WebStoreContext>();
            services.AddScoped<KmStoreContext>();

            services.AddScoped<IExternalUsersService, FirebaseExternalUsersService>();
            services.AddTransient<IValidator<CartTransactionApiModel>, CartTransactionApiModelValidator>();
            services.AddSingleton<IRateLimiter, RateLimiter>();
            services.AddScoped<IKmOrderService>(sp => new KmOrderService(
                sp.GetRequiredService<IRepository<KmOrder>>(),
                sp.GetRequiredService<IExternalUsersService>(),
                sp.GetRequiredService<IWorkContext>(),
                sp.GetRequiredService<IStoreService>(),
                sp.GetRequiredService<KmStoreContext>(),
                sp.GetRequiredService<ICustomerService>(),
                sp.GetRequiredService<IAddressService>(),
                sp.GetRequiredService<IPaymentService>(),
                sp.GetRequiredService<IOrderProcessingService>(),
                sp.GetRequiredService<IProductService>(),
                sp.GetRequiredService<IStoreMappingService>(),
                sp.GetRequiredService<IShoppingCartService>(),
                sp.GetRequiredService<ISystemClock>(),
                sp.GetRequiredService<ILogger>()));

            services.AddScoped<IOrderDocumentStore, OrderDocumentStore>();
            services.AddScoped<IUserProfileDocumentStore, UserProfileDocumentStore>();
            services.AddScoped(typeof(IDocumentStore<>), typeof(FirebaseDocumentStore<>));

            services.AddSignalR();
        }

        public void Configure(IApplicationBuilder application)
        {
            application.UseCors(CorsPolicy);
        }

        public int Order => 10;
    }
}