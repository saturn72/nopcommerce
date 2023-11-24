using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Plugins;

namespace Km.Catalog;

public class Plugin : BasePlugin
{
    private readonly IScheduleTaskService _scheduleTaskService;

    private const string CatalogTaskName = "Export catalog to kedem-market's firebase";

    public Plugin(IScheduleTaskService scheduleTaskService)
    {
        _scheduleTaskService = scheduleTaskService;
    }

    public override Task InstallAsync()
    {
        var catalogTask = new ScheduleTask
        {
            Enabled = true,
            Name = CatalogTaskName,
            Seconds = 60,
            StopOnError = false,
            Type = typeof(UpdateCatalogTask).FullName
        };
        _scheduleTaskService.InsertTaskAsync(catalogTask);
        return base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var all = await _scheduleTaskService.GetAllTasksAsync(true);

        var tasks = all.Where(t => t.Name == CatalogTaskName);
        foreach (var item in tasks)
            await _scheduleTaskService.DeleteTaskAsync(item);

        await base.UninstallAsync();
    }
}
