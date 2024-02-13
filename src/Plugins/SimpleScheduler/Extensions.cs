using Microsoft.Extensions.Configuration;
using SimpleScheduler;

namespace Microsoft.Extensions.DependencyInjection;

public static class SimpleSchedulerExtensions
{
    public static void AddSimpleScheduler(this IServiceCollection services)
    {
        services.AddSingleton<IScheduler, SimpleScheduler.Scheduler>();
    }
}
