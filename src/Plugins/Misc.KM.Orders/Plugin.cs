using Nop.Core.Domain.ScheduleTasks;
using Nop.Plugin.Misc.KM.Orders.ScheduleTasks;
using Nop.Services.Plugins;
using Nop.Services.ScheduleTasks;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.KM.Orders
{
    public class Plugin : BasePlugin
    {
        private readonly IScheduleTaskService _scheduleTaskService;
        private const string TaskName = "Import orders from Firebase";

        public Plugin(IScheduleTaskService scheduleTaskService)
        {
            _scheduleTaskService = scheduleTaskService;
        }

        public override Task InstallAsync()
        {
            var task = new ScheduleTask
            {
                Enabled = true,
                Name = TaskName,
                Seconds = 60,
                StopOnError = false,
                Type = typeof(ImportOrderScheduleTask).FullName
            };
            _scheduleTaskService.InsertTaskAsync(task);
            return base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            var all = await _scheduleTaskService.GetAllTasksAsync(true);

            var tasks = all.Where(t => t.Name == TaskName);
            foreach (var item in tasks)
                await _scheduleTaskService.DeleteTaskAsync(item);
            await base.UninstallAsync();
        }
    }
}
