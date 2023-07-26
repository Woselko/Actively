using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using AutoMapper;

namespace ActivelyApp.Mappings
{
    public class PlayerMappingProfile : Profile
    {
        public PlayerMappingProfile()
        {
            CreateMap<CreatePlayerInfo, Player>()
                .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(x => x.NickName, opt => opt.MapFrom(src => src.NickName));

            CreateMap<UpdatePlayerInfo, Player>()
                .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(x => x.NickName, opt => opt.MapFrom(src => src.NickName));


            CreateMap<Player, PlayerDto>()
                .ForMember(x => x.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(x => x.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(x => x.NickName, opt => opt.MapFrom(src => src.NickName))
                .ForMember(x => x.Games, opt => opt.MapFrom(src => src.Games));

        }
    }
}
