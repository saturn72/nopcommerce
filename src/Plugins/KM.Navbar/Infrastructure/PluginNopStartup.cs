using KM.Navbar.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Nop.Core.Infrastructure;

namespace KM.Navbar.Infrastructure;

public class PluginNopStartup : INopStartup
{

    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<INavbarService, NavbarService>();
    }

    public void Configure(IApplicationBuilder application)
    {
    }

    public int Order => 10;
}