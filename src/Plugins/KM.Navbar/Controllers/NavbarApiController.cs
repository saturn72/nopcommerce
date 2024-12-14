
namespace KM.Navbar.Controllers;

[Route("api/navbar")]
public class NavbarApiController : ControllerBase
{
    private readonly INavbarService _navbarService;

    public NavbarApiController(INavbarService navbarService)
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
