using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using AutoMapper;

namespace ActivelyApp.Mappings
{
    public class GameMappingProfile : Profile
    {
        public GameMappingProfile()
        {
            CreateMap<CreateGameInfo, Game>()
                .ForMember(x => x.GameTime, opt => opt.MapFrom(src => src.GameTime))
                .ForMember(x => x.Sport, opt => opt.MapFrom(src => src.Sport));

            CreateMap<UpdateGameInfo, Game>()
                .ForMember(x => x.GameTime, opt => opt.MapFrom(src => src.GameTime));

            CreateMap<Game, GameDto>()
                .ForMember(x => x.GameTime, opt => opt.MapFrom(src => src.GameTime))
                .ForMember(x => x.Sport, opt => opt.MapFrom(src => src.Sport))
                .ForMember(x => x.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(x => x.Players, opt => opt.MapFrom(src => src.Players));

            CreateMap<GameDto, Game>()
                .ForMember(x => x.GameTime, opt => opt.MapFrom(src => src.GameTime))
                .ForMember(x => x.Sport, opt => opt.MapFrom(src => src.Sport))
                .ForMember(x => x.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(x => x.Players, opt => opt.MapFrom(src => src.Players));
        }
    }
}
