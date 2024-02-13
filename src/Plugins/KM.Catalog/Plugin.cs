using KM.Catalog.Components;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Cms;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Km.Catalog;

public class Plugin : BasePlugin, IWidgetPlugin
{
    private readonly IScheduleTaskService _scheduleTaskService;
    protected readonly ISettingService _settingService;
    private readonly WidgetSettings _widgetSettings;

    private const string SystemName = "KM.Catalog";
    private const string CatalogTaskName = "Export catalog to kedem-market's firebase";

    public bool HideInWidgetList => true;

    public Plugin(
        IScheduleTaskService scheduleTaskService,
        ISettingService settingService,
        WidgetSettings widgetSettings
        )
    {
        _scheduleTaskService = scheduleTaskService;
        _widgetSettings = widgetSettings;
        _settingService = settingService;
    }

    public override async Task InstallAsync()
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

        if (!_widgetSettings.ActiveWidgetSystemNames.Contains(SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Add(SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var all = await _scheduleTaskService.GetAllTasksAsync(true);

        var tasks = all.Where(t => t.Name == CatalogTaskName);
        foreach (var item in tasks)
            await _scheduleTaskService.DeleteTaskAsync(item);


        //settings
        if (_widgetSettings.ActiveWidgetSystemNames.Contains(SystemName))
        {
            _widgetSettings.ActiveWidgetSystemNames.Remove(SystemName);
            await _settingService.SaveSettingAsync(_widgetSettings);
        }

        await base.UninstallAsync();
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string> { AdminWidgetZones.StoreDetailsBottom });
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        return typeof(WidgetsStoreAddressViewComponent);
    }
}
