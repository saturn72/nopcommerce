using KedemMarket.Api.Models.User;
using Nop.Core;
using Nop.Services.Vendors;

namespace KedemMarket.Api.Controllers;

[Route("api/store")]
public class StoreController : KmApiControllerBase
{
    private readonly IWorkContext _workContext;
    private readonly IVendorService _vendorService;
    private readonly IVendorApiModelFactory _vendorApiModelFactory;

    public StoreController(
        IWorkContext workContext,
        IVendorService vendorService,
        IVendorApiModelFactory vendorApiModelFactory)
    {
        _workContext = workContext;
        _vendorService = vendorService;
        _vendorApiModelFactory = vendorApiModelFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetVendorsAsync([FromQuery] int pageSize = 25, [FromQuery] int offset = 0)
    {
        var v1 = await _vendorService.GetAllVendorsAsync(pageIndex: offset / pageSize, pageSize: pageSize);
        var vvs = v1
            .Where(v => v.Active && !v.Deleted)
            .ToList();

        var stores = new List<object>();
        foreach (var v in vvs)
            stores.Add(await _vendorApiModelFactory.ToVendorApiModel(v));

        return ToJsonResult(new { stores });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetCurrentUserStore()
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var vendors = await _vendorService.GetVendorsByCustomerIdsAsync([customer.Id]);
        if (vendors == default || vendors.Count == 0)
            return Ok();

        var model = await _vendorApiModelFactory.ToVendorApiModel(vendors.ElementAt(0));
        return ToJsonResult(model);
    }
    [HttpPut]
    public async Task<IActionResult> CreateOrUpdateStoreAsync([FromBody] VendorApiModel model)
    {
        var customer = await _workContext.GetCurrentCustomerAsync();
        var vendors = await _vendorService.GetVendorsByCustomerIdsAsync([customer.Id]);
        
        var cur = vendors?.ElementAtOrDefault(0);
        //create
        if (vendors == default || vendors.Count == 0)
            return Ok();
        //update

        throw new NotImplementedException();
    }

}