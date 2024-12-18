using AutoMapper;
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
    }
    public int Order => 1;
}