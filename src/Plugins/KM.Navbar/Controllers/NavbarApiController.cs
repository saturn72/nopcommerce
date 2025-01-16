
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace KM.Navbar.Controllers;

[Route("api/navbar")]
public class NavbarApiController : ControllerBase
{
    private readonly KM.Navbar.Factories.INavbarFactory _navbarFactory;
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };

    public NavbarApiController(KM.Navbar.Factories.INavbarFactory navbarFactory)
    {
        _navbarFactory = navbarFactory;
    }

    protected internal static JsonResult ToJsonResult(object body)
    {
        return new(body, _jsonSerializerSettings);
    }
    [HttpGet("{name}")]
    public async Task<IActionResult> GeNavbarInfoByNameAsync(string name)
    {
        if (name.HasNoValue())
            return BadRequest();

        var data = await _navbarFactory.PrepareNavbarApiModelByNameAsync(name);
        if (data == null)
            return BadRequest();
        return ToJsonResult(data);
    }
}
