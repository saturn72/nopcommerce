namespace KM.Navbar.Infrastructure;

public class PluginNopStartup : INopStartup
{

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INavbarService, NavbarService>();
        services.AddScoped<INavbarFactory, NavbarFactory>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 10;
}