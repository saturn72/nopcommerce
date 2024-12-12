using KM.Navbar.Services;

namespace KM.Navbar.Controllers;

[Route("api/navbar")]
public class NavbarController : ControllerBase
{
    private readonly INavbarService _navbarService;

    public NavbarController(INavbarService navbarService)
    {
        _navbarService = navbarService;
    }
    [HttpGet]
    public async Task<IActionResult> GetNavbarAsync()
    {
        var data = await _navbarService.GetNavbarInfoAsync();
        return Ok(data);
    }
}
