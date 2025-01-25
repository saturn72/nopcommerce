using KM.Common.Services.Media;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nop.Core.Infrastructure;

namespace KM.Common.Infrastructure;
public class NopStartup : INopStartup
{
    public int Order => 100;

    public void Configure(IApplicationBuilder application)
    {
    }

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
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
}
