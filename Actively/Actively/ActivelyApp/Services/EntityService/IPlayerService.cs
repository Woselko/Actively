using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;

namespace ActivelyApp.Services.EntityService
{
    public interface IPlayerService
    {
        Task Create(CreatePlayerInfoDto newPlayer);
        Task Delete(int id);
        Task<IEnumerable<PlayerDto>> GetAll();
        Task<PlayerDto> GetById(int id);
        Task Update(UpdatePlayerInfoDto player, int id);
    }
}