using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using AutoMapper;

namespace ActivelyApp.Mappings
{
    public class GameMappingProfile : Profile
    {
        public GameMappingProfile()
        {
            CreateMap<CreateGameInfoDto, Game>();
            CreateMap<UpdateGameInfoDto, Game>();
            CreateMap<Game, GameDto>();
            CreateMap<GameDto, Game>();
        }
    }
}
