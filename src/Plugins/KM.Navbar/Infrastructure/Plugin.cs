using Nop.Core.Domain.Localization;
using Nop.Services.Localization;

namespace KM.Navbar.Infrastructure;
public class Plugin : BasePlugin
{
    private readonly ILocalizationService _localizationService;
    private readonly ILanguageService _languageService;
    private readonly IEnumerable<LocaleStringResource> _localeStringResources;

    public Plugin(
        ILocalizationService localizationService,
        ILanguageService languageService)
    {
        _localizationService = localizationService;
        _languageService = languageService;
        _localeStringResources = GetLocaleStringResources();
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
        };
    }

    public async override Task InstallAsync()
    {
        await base.InstallAsync();
        await InsertLocaleResourcesAsync();
    }


    public async override Task UninstallAsync()
    {
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
}
