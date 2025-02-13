namespace KedemMarket.Factories.Vendors;
public interface IVendorApiModelFactory
{
    Task<VendorApiModel> ToVendorApiModel(Vendor vendor);
}
