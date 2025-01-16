using FluentValidation;
using KM.Common.Services.Media;
using KM.Navbar.Admin.Validators;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace KM.Navbar.Infrastructure;

public class PluginNopStartup : INopStartup
{

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INavbarService, NavbarService>();
        services.AddScoped<KM.Navbar.Admin.Factories.INavbarFactory, KM.Navbar.Admin.Factories.NavbarFactory>();
        services.AddScoped<KM.Navbar.Factories.INavbarFactory, KM.Navbar.Factories.NavbarFactory>();
        services.AddScoped<Admin.Factories.INavbarFactory, Admin.Factories.NavbarFactory>();
        services.AddTransient<IValidator<Admin.Models.NavbarInfoModel>, NavInfoModelValidator>();
        services.AddTransient<IValidator<Admin.Models.CreateOrUpdateNavbarElementModel>, CreateNavbarElementPopupModelValidator>();
        services.TryAddSingleton<MediaConvertor>();
        services.TryAddScoped<IStorageManager, GcpStorageManager>();

    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 10;
}