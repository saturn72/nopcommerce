using AutoMapper;
using KM.Navbar.Admin.Models;
using Nop.Core.Infrastructure.Mapper;

namespace KM.Navbar.Infrastructure;

public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    public MapperConfiguration()
    {
        CreateMap<Admin.Models.NavbarInfoModel, NavbarInfo>()
              .ReverseMap();
        CreateMap<Admin.Models.NavbarElementModel, NavbarElement>()
              .ReverseMap();
        CreateMap<NavbarElement, Models.NavbarElementModel>()
              .ReverseMap();

        CreateMap<Admin.Models.CreateOrUpdateNavbarElementModel, NavbarElement>()
             .ReverseMap();
        CreateMap<Admin.Models.NavbarElementVendorModel, NavbarElementVendor>()
            .ReverseMap();

        CreateMap<Nop.Core.Domain.Vendors.Vendor, NavbarElementVendorModel>()
            .ForMember(dest => dest.VendorName, mo => mo.MapFrom(src => src.Name))
            .ForMember(dest => dest.VendorId, mo => mo.MapFrom(src => src.Id));
    }
    public int Order => 1;
}