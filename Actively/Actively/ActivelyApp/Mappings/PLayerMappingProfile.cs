using ActivelyApp.Models.Player;
using ActivelyDomain.Entities;
using AutoMapper;

namespace ActivelyApp.Mappings
{
    public class PLayerMappingProfile : Profile
    {
        public PLayerMappingProfile()
        {
            CreateMap<CreatePlayerInfo, Player>()
                .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(x => x.NickName, opt => opt.MapFrom(src => src.NickName));
        }
    }
}
