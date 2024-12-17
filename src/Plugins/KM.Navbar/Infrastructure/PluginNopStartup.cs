namespace KM.Navbar.Infrastructure;

public class PluginNopStartup : INopStartup
{

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INavbarInfoService, NavbarInfoService>();
        services.AddScoped<INavbarFactory, NavbarFactory>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 10;
}