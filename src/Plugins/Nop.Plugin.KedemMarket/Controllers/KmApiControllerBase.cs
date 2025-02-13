
namespace KedemMarket.Controllers;

[ApiController]
[Area("api")]
public class KmApiControllerBase : ControllerBase
{

    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver()
    };
    protected internal static JsonResult ToJsonResult(object body)
    {
        return new(body, _jsonSerializerSettings);
    }
}
