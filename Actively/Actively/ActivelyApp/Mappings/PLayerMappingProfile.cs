using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;
using AutoMapper;

namespace ActivelyApp.Mappings
{
    public class PlayerMappingProfile : Profile
    {
        public PlayerMappingProfile()
        {
            CreateMap<CreatePlayerInfoDto, Player>();
            CreateMap<UpdatePlayerInfoDto, Player>();
            CreateMap<Player, PlayerDto>();
        }
    }
}
