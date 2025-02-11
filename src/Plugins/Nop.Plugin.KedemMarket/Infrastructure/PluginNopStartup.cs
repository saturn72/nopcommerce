using FluentValidation;
using KedemMarket.Admin.Factories;
using KedemMarket.Admin.Models;
using KedemMarket.Admin.Validators;
using KedemMarket.Infrastructure;
using KedemMarket.Factories.Catalog;
using KedemMarket.Services.Media;
using Microsoft.Extensions.DependencyInjection.Extensions;
using KedemMarket.Services.Navbar;

namespace KedemMarket.Infrastructure;

public class PluginNopStartup : INopStartup
{

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        //new KM.Common.Infrastructure.NopStartup().ConfigureServices(services, configuration);
        services.AddScoped<INavbarService, NavbarService>();
        services.AddScoped<INavbarFactory, NavbarFactory>();
        services.AddScoped<KedemMarket.Factories.Navbar.INavbarFactory, KedemMarket.Factories.Navbar.NavbarFactory>();
        services.AddScoped<INavbarFactory, NavbarFactory>();
        services.AddTransient<IValidator<NavbarInfoModel>, NavInfoModelValidator>();
        services.AddTransient<IValidator<CreateOrUpdateNavbarElementModel>, CreateNavbarElementPopupModelValidator>();

        services.TryAddScoped<IProductApiFactory, ProductApiFactory>();
        services.TryAddSingleton<MediaConvertor>();

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
    }

    public int Order => 10;
}