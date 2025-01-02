using KM.Navbar.Widgets;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Localization;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Web.Framework.Infrastructure;

namespace KM.Navbar.Infrastructure;
public class Plugin : BasePlugin, IWidgetPlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly ILanguageService _languageService;
    private readonly IEnumerable<LocaleStringResource> _localeStringResources;
    private readonly WidgetSettings _widgetSettings;
    private readonly ISettingService _settingService;

    public bool HideInWidgetList => true;

    public Plugin(
        ILocalizationService localizationService,
        ILanguageService languageService,
        WidgetSettings widgetSettings,
        ISettingService settingService)
    {
        _localizationService = localizationService;
        _languageService = languageService;
        _localeStringResources = GetLocaleStringResources();
        _widgetSettings = widgetSettings;
        _settingService = settingService;
    }

    private IEnumerable<LocaleStringResource> GetLocaleStringResources()
    {
        var languageId = _languageService.GetAllLanguages(showHidden: true)
          .Where(l => l.Published)
          .OrderBy(l => l.DisplayOrder).First().Id;
        return new[]
       {
        new LocaleStringResource
        {
                LanguageId = languageId,
                ResourceName = "admin.navbar.list",
                ResourceValue = "Navbars list"
            },
        new LocaleStringResource
        {
                LanguageId = languageId,
                ResourceName = "admin.navbars.list.searchnavbarname",
                ResourceValue = "Name"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "admin.navbars.list.searchpublished",
                ResourceValue = "Published"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.List.SearchStore",
                ResourceValue = "Store"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.List.SearchStore",
                ResourceValue = "Store"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName ="Admin.Navbar.List.SearchPublished.All",
              ResourceValue = "All"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName ="Admin.Navbar.List.SearchPublished.PublishedOnly",
              ResourceValue = "Published Only"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName ="Admin.Navbar.List.SearchPublished.UnpublishedOnly",
                ResourceValue = "Unpublished Only"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbar.Fields.Name",
                ResourceValue = "Name"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbar.Fields.Name.Required",
                ResourceValue = "Name required"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbar.Fields.Name.Unique",
                ResourceValue = "Navbar with same name already exist. Navbar names must be unique"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbar.Fields.Elements",
                ResourceValue = "Elements"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbar.Fields.DisplayOrder",
                ResourceValue = "Display Order"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbar.Fields.Published",
                ResourceValue = "Published"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Fields.Description",
                ResourceValue = "Published"
            },

            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Fields.PageSize",
                ResourceValue = "Published"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Fields.AllowCustomersToSelectPageSize",
                ResourceValue = "Allow Customers To Select Page Size"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Fields.PageSizeOptions",
                ResourceValue = "Page Size Options"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Fields.Deleted",
                ResourceValue = "Deleted"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Fields.LimitedToStores",
                ResourceValue = "Limited To Stores"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.AddNew",
                ResourceValue = "Add new Navbar"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Elements",
                ResourceValue = "Navbar Elements"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Added",
                ResourceValue = "Navbar was added"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Failed",
                ResourceValue = "Failed to add navbar"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Deleted",
                ResourceValue = "Navbar deleted"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.EditNavbarDetails",
                ResourceValue = "Edit Navbar Details"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Elements.AddNew",
                ResourceValue = "Add Navbar Element"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Elements.Added",
                ResourceValue = "Navbar Element Added successfuly"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Elements.Updated",
                ResourceValue = "Navbar Element Updated successfuly"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.Navbars.Elements.Edit",
                ResourceValue = "Edit Navbar Element"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Alt",
                ResourceValue = "Alt"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Icon",
                ResourceValue = "Icon"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Icon.Required",
                ResourceValue = "Icon is required"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.ActiveIcon",
                ResourceValue = "ActiveIcon"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.ActiveIcon.Required",
                ResourceValue = "Active Icon is required"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Type",
                ResourceValue = "Type"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Type.Invalid",
                ResourceValue = "Value is invalid"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Type.Required",
                ResourceValue = "Value is required"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Value.Required",
                ResourceValue = "Value is required"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.NavbarInfoId.Required",
                ResourceValue = "Navbar Id is required"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Index",
                ResourceValue = "Index"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Caption",
                ResourceValue = "Caption"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Caption.Required",
                ResourceValue = "Caption is required"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Caption.Unique",
                ResourceValue = "Caption should be unique for navbar"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Tags",
                ResourceValue = "Tags"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Type",
                ResourceValue = "Type"
            },
            new LocaleStringResource
            {
                LanguageId = languageId,
                ResourceName = "Admin.NavbarElement.Fields.Value",
                ResourceValue = "Value"
            },
        };
    }

    public async override Task InstallAsync()
    {
        await base.InstallAsync();
        await InsertLocaleResourcesAsync();
        _widgetSettings.ActiveWidgetSystemNames.Add(PluginDescriptor.SystemName);
        await _settingService.SaveSettingAsync(_widgetSettings);
    }

    public async override Task UninstallAsync()
    {
        _widgetSettings.ActiveWidgetSystemNames.Remove(PluginDescriptor.SystemName);
        await _settingService.SaveSettingAsync(_widgetSettings);
        await UninsertLocaleResourcesAsync();
        await base.UninstallAsync();
    }

    private async Task UninsertLocaleResourcesAsync()
    {
        var rns = _localeStringResources.Select(l => l.ResourceName).ToList();
        await _localizationService.DeleteLocaleResourcesAsync(rns);
    }

    private async Task InsertLocaleResourcesAsync()
    {
        var tasks = new List<Task>();
        foreach (var lr in _localeStringResources)
            tasks.Add(_localizationService.InsertLocaleStringResourceAsync(lr));
        await Task.WhenAll(tasks);
    }

    public Task<IList<string>> GetWidgetZonesAsync()
    {
        return Task.FromResult<IList<string>>(new List<string>
        {
            AdminWidgetZones.VendorDetailsBlock
        });
    }

    public Type GetWidgetViewComponent(string widgetZone)
    {
        if (widgetZone == AdminWidgetZones.VendorDetailsBlock)
            return typeof(VendorAddRemoveTags);

        throw new ArgumentOutOfRangeException(widgetZone);
    }
}
