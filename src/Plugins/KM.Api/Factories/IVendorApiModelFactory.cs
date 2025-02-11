using KedemMarket.Api.Models.User;

namespace KedemMarket.Api.Factories;
public interface IVendorApiModelFactory
{
    Task<VendorApiModel> ToVendorApiModel(Vendor vendor);
}
