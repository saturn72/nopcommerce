
namespace KM.Navbar.Controllers;

[Route("api/navbar")]
public class NavbarApiController : ControllerBase
{
    private readonly INavbarInfoService _navbarService;

    public NavbarApiController(INavbarInfoService navbarService)
    {
        _navbarService = navbarService;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetNavbarInfoByNameAsync(string name)
    {
        var data = await _navbarService.GetNavbarInfoByNameAsync(name);
        return Ok(data);
    }
}
