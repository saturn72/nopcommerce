using KM.Navbar.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace KM.Navbar.Controllers;

[Route("api/navbar")]
public class NavbarApiController : ControllerBase
{
    private readonly INavbarInfoService _navbarService;
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public NavbarApiController(INavbarInfoService navbarService)
    {
        _navbarService = navbarService;
    }

    protected internal static JsonResult ToJsonResult(object body)
    {
        return new(body, _jsonSerializerSettings);
    }
    [HttpGet("{name}")]
    public async Task<IActionResult> GetNavbarInfoByNameAsync(string name)
    {
        var navbar = await _navbarService.GetNavbarInfoByNameAsync(name);
        var elements = navbar?.Elements ?? [];

        var data = elements.Select(ne => new NavbarElementModel
        {
            ActiveIcon = ne.ActiveIcon,
            Alt = ne.Alt,
            Caption = ne.Caption,
            Icon = ne.Icon,
            Index = ne.Index,
            Tags = ne.Tags,
            Type = ne.Type,
            Value = ne.Value,
        }).ToList();
        return ToJsonResult(data);
    }
}
