using ActivelyApp.Models.Player;
using ActivelyDomain.Entities;

namespace ActivelyApp.Services.EntityService
{
    public interface IPlayerService
    {
        Task Create(CreatePlayerInfo newPlayer);
        Task Delete(int id);
        Task<IEnumerable<Player>> GetAll();
        Task<Player> GetById(int id);
        Task Update(UpdatePlayerInfo player);
    }
}