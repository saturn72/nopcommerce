using AutoMapper;
using KedemMarket.Admin.Models.Navbar;
using Nop.Core.Infrastructure.Mapper;

namespace KedemMarket.Infrastructure;

public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    public MapperConfiguration()
    {
        CreateMap<NavbarInfoModel, NavbarInfo>()
              .ReverseMap();
        CreateMap<Admin.Models.Navbar.NavbarElementModel, NavbarElement>()
              .ReverseMap();
        CreateMap<NavbarElement, KedemMarket.Models.Navbar.NavbarElementModel>()
              .ReverseMap();

        CreateMap<CreateOrUpdateNavbarElementModel, NavbarElement>()
             .ReverseMap();
        CreateMap<NavbarElementVendorModel, NavbarElementVendor>()
            .ReverseMap();

        CreateMap<Nop.Core.Domain.Vendors.Vendor, NavbarElementVendorModel>()
            .ForMember(dest => dest.VendorName, mo => mo.MapFrom(src => src.Name))
            .ForMember(dest => dest.VendorId, mo => mo.MapFrom(src => src.Id));

        CreateMap<ProductInfoApiModel, ProductSlimApiModel>();
    }
    public int Order => 1;
}