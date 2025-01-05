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
             .ReverseMap();

        CreateMap<Admin.Models.CreateOrUpdateNavbarElementModel, NavbarElement>()
            .ReverseMap();
    }
    public int Order => 1;
}