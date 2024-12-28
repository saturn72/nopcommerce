using FluentValidation;
using KM.Navbar.Admin.Validators;

namespace KM.Navbar.Infrastructure;

public class PluginNopStartup : INopStartup
{

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INavbarInfoService, NavbarInfoService>();
        services.AddScoped<INavbarFactory, NavbarFactory>();
        services.AddTransient<IValidator<NavbarInfoModel>, NavInfoModelValidator>();
        services.AddTransient<IValidator<CreateOrUpdateNavbarElementPopupModel>, CreateNavbarElementPopupModelValidator>();

    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 10;
}