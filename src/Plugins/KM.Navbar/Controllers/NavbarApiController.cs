
namespace KM.Navbar.Controllers;

[Route("api/navbar")]
public class NavbarApiController : ControllerBase
{
    private readonly INavbarInfoService _navbarService;

    public NavbarApiController(INavbarInfoService navbarService)
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
