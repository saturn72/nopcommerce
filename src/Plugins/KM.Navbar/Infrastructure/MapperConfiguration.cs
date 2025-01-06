using AutoMapper;
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
            //.ForMember(dest =>dest.Vendors, mo => mo.MapFrom(src => src.Vendors))
             .ReverseMap();
    }
    public int Order => 1;
}