
using KM.Catalog.Factories;

namespace KM.Catalog.Infrastructure;

public class PluginNopStartup : INopStartup
{
    private const string CatalogWSCorsPolicy = "catalog-cors";

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.ViewLocationExpanders.Add(new ViewLocationExpander());
        });

        var origins = configuration.GetRequiredSection("cors:origins")
                    .AsEnumerable()
                    .Where(o => o.Value != default && new Uri(o.Value) != null)
                    .Select(o => o.Value)
                    .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(name: CatalogWSCorsPolicy,
                          policy => policy.WithOrigins(origins)
                              .WithMethods("GET", "POST")
                              .AllowCredentials()
                              .AllowAnyHeader());
        });

        //register services and interfaces
        services.AddScoped<IMediaItemInfoService, MediaItemInfoService>();
        services.AddSingleton<IStorageManager, GcpStorageManager>();
        //services.AddScoped(typeof(IStore<>), typeof(FirestoreStore<>));

        services.Configure<GcpOptions>(options =>
        {
            var bn = configuration["gcpOptions:bucketName"];
            if (string.IsNullOrEmpty(bn) || string.IsNullOrWhiteSpace(bn))
                throw new ArgumentException(nameof(GcpOptions.BucketName));
            options.BucketName = bn;
        });

        services.AddScoped<IKmCatalogModelFactory, KmCatalogModelFactory>();
        services.AddSignalR();
    }

    public void Configure(IApplicationBuilder application)
    {
        application.UseCors(CatalogWSCorsPolicy);
    }

    public int Order => 10;
}