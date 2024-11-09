using KM.Api.Models.User;

namespace KM.Api.Factories;
public interface IVendorApiModelFactory
{
    Task<VendorApiModel> ToVendorApiModel(Vendor vendor);
}
