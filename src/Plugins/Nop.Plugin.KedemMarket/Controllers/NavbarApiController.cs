using KedemMarket.Api.Controllers;

namespace KedemMarket.Controllers;

[Route("api/navbar")]
public class NavbarApiController : KmApiControllerBase
{
    private readonly KedemMarket.Factories.Navbar.INavbarFactory _navbarFactory;

    public NavbarApiController(KedemMarket.Factories.Navbar.INavbarFactory navbarFactory)
    {
        _navbarFactory = navbarFactory;
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
