using ActivelyApp.Models.Entity;
using ActivelyDomain.Entities;
using ActivelyInfrastructure;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace ActivelyApp.Mappings
{
    public class GameMappingProfile : Profile
    {
        public GameMappingProfile()
        {
            CreateMap<CreateGameInfo, Game>()
                .ForMember(x => x.GameTime, opt => opt.MapFrom(src => src.GameTime))
                .ForMember(x => x.GameDate, opt => opt.MapFrom(src => src.GameDate))
                .ForMember(x => x.Sport, opt => opt.MapFrom(src => src.Sport));
        }
    }
}
