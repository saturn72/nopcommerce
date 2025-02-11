using KedemMarket.Common.Factories;
using KedemMarket.Common.Services.Media;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KedemMarket.Common.Infrastructure;
public class CommonServiceConfigurar
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.TryAddScoped<IStorageManager, GcpStorageManager>();
        services.TryAddScoped<IProductApiFactory, ProductApiFactory>();
        services.TryAddSingleton<MediaConvertor>();

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
}
