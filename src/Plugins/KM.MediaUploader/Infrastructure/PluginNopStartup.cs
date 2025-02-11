
namespace KedemMarket.MediaUploader.Infrastructure;

public class PluginNopStartup : INopStartup
{

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<IStorageManager, GcpStorageManager>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 10;
}