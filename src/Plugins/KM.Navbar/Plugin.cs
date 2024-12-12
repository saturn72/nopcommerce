
namespace KM.Navbar;

public class Plugin : BasePlugin
{
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly ScheduleTask[] _tasks;


    public Plugin(IScheduleTaskService scheduleTaskService)
    {
        //_scheduleTaskService = scheduleTaskService;
        //_tasks = new[]
        //{
        //    new ScheduleTask
        //    {
        //        Enabled = true,
        //        Name =  "Import orders from Firebase",
        //        Seconds = 60,
        //        StopOnError = false,
        //        Type = typeof(ImportOrderScheduleTask).FullName
        //    },
        //};
    }
}
