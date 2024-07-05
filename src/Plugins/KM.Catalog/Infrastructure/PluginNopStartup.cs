
using KM.Catalog.EventConsumers;
using SimpleScheduler;

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

        services.AddScoped<IStructuredDataService, StructuredDataService>();
        services.AddScoped<IDocumentStore, FirestoreDocumentStore>();

        services.Configure<GcpOptions>(options =>
        {
            var bn = configuration["gcpOptions:bucketName"];
            if (string.IsNullOrEmpty(bn) || string.IsNullOrWhiteSpace(bn))
                throw new ArgumentException(nameof(GcpOptions.BucketName));
            options.BucketName = bn;
        });

        services.AddSignalR();
        services.AddSimpleScheduler();
        services.AddScoped<ProductActivationHandler>();
    }

    public void Configure(IApplicationBuilder application)
    {
        _ = Task.Run(async () =>
        {
            var services = application.ApplicationServices;
            var pah = services.GetService<ProductActivationHandler>();
            await pah.SetNextPageAsync(DateTime.UtcNow);
        });

        application.UseCors(CatalogWSCorsPolicy);
    }

    public int Order => 10;
}