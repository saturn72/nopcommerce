
using KM.Api.ScheduleTasks;

namespace KM.Api;

public class Plugin : BasePlugin
{
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly ScheduleTask[] _tasks;


    public Plugin(IScheduleTaskService scheduleTaskService)
    {
        _scheduleTaskService = scheduleTaskService;
        _tasks = new[]
        {
            new ScheduleTask
            {
                Enabled = true,
                Name =  "Import orders from Firebase",
                Seconds = 60,
                StopOnError = false,
                Type = typeof(ImportOrderScheduleTask).FullName
            },
        };
    }

    public override Task InstallAsync()
    {
        foreach (var task in _tasks)
            _scheduleTaskService.InsertTaskAsync(task);
        return base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var all = await _scheduleTaskService.GetAllTasksAsync(true);

        var tn = _tasks.Select(t => t.Name).ToList();
        var ttds = all.Where(t => tn.Contains(t.Name));

        foreach (var ttd in ttds)
            await _scheduleTaskService.DeleteTaskAsync(ttd);
        await base.UninstallAsync();
    }
}
