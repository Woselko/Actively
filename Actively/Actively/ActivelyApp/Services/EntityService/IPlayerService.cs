using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;

namespace ActivelyApp.Services.EntityService
{
    public interface IPlayerService
    {
        Task Create(CreatePlayerInfo newPlayer);
        Task Delete(int id);
        Task<IEnumerable<PlayerDto>> GetAll();
        Task<PlayerDto> GetById(int id);
        Task Update(UpdatePlayerInfo player, int id);
    }
}