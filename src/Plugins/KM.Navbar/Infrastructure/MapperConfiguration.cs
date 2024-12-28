using AutoMapper;
using KM.Navbar.Models;
using Nop.Core.Infrastructure.Mapper;

namespace KM.Navbar.Infrastructure;

public class MapperConfiguration : Profile, IOrderedMapperProfile
{
    public MapperConfiguration()
    {
        CreateMap<NavbarInfoModel, NavbarInfo>()
              .ReverseMap();
        CreateMap<NavbarElementModel, NavbarElement>()
              .ReverseMap();
        CreateMap<NavbarElement, NavbarElementSlimModel>()
              .ReverseMap();

        CreateMap<CreateOrUpdateNavbarElementPopupModel, NavbarElement>()
             .ReverseMap();

    }
    public int Order => 1;
}