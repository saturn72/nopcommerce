using KM.Navbar.Admin.Domain;

namespace KM.Navbar.Admin.Factories;

public class NavbarFactory : INavbarFactory
{
    private readonly IRepository<NavbarInfo> _navbarInfoRepository;

    public NavbarFactory(IRepository<NavbarInfo> navbarInfoRepository)
    {
        _navbarInfoRepository = navbarInfoRepository;
    }

    public async Task<NavbarInfoSearchModel> PrepareNavbarListModelAsync()
    {
        return new NavbarInfoSearchModel
        {

        };
        var nis = await _navbarInfoRepository.GetAllAsync(x => x, cks => cks.PrepareKey(new("navbar-infos")));

        var d = await nis.Select(g => new NavbarInfoModel
        {
            Id = g.Id,
            Name = g.Name,
            Index = g.Index,
            Published = g.Published,
            Elements = PrepareNavbarElements(g.Elements)
        }).OrderBy(x => x.Index)
        .ToListAsync();

    }

    public IList<NavbarElementModel> PrepareNavbarElements(IEnumerable<NavbarElement> elements)
    {
        return elements.Select(e => new NavbarElementModel
        {
            Alt = e.Alt,
            Icon = e.Icon,
            Index = e.Index,
            Label = e.Label,
            Tags = e.Tags,
            Type = e.Type.ToString().ToLower(),
            Value = e.Value,
        }).ToList();
    }
}
