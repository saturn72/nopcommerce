using Nop.Core.Domain.ScheduleTasks;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace KM.Catalog;

public class Plugin : BasePlugin
{
    private readonly IScheduleTaskService _scheduleTaskService;
    private readonly ILocalizationService _localizationService;
    private readonly IWebHelper _webHelper;
    private readonly ISettingService _settingService;
    private const string CatalogTaskName = "Export catalog to kedem-market's firebase";

    public Plugin(
        IWebHelper webHelper,
        IScheduleTaskService scheduleTaskService,
        ILocalizationService localizationService,
        ISettingService settingService)
    {
        _webHelper = webHelper;
        _scheduleTaskService = scheduleTaskService;
        _localizationService = localizationService;
        _settingService = settingService;
    }

    public override async Task InstallAsync()
    {
        await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
        {
            { "KM.Catalog.DefaultVendor", "Default Vendor" },
            { "KM.Catalog.Configuration", "Kedem Market Configuration" },
        });
        await _settingService.SaveSettingAsync(new KmCatalogSettings());

        var catalogTask = new ScheduleTask
        {
            Enabled = true,
            Name = CatalogTaskName,
            Seconds = 60,
            StopOnError = false,
            Type = typeof(UpdateCatalogTask).FullName
        };
        await _scheduleTaskService.InsertTaskAsync(catalogTask);

        await base.InstallAsync();
    }

    public override async Task UninstallAsync()
    {
        var all = await _scheduleTaskService.GetAllTasksAsync(true);

        var tasks = all.Where(t => t.Name == CatalogTaskName);
        foreach (var item in tasks)
            await _scheduleTaskService.DeleteTaskAsync(item);

        await _localizationService.DeleteLocaleResourceAsync("KM.Catalog.DefaultVendor");

        await _settingService.DeleteSettingAsync<KmCatalogSettings>();

        await base.UninstallAsync();
    }

    public override string GetConfigurationPageUrl()
    {
        return $"{_webHelper.GetStoreLocation()}Admin/KmCatalog/Configure";
    }
}
