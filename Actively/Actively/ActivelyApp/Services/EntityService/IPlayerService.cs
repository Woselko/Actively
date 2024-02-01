using ActivelyApp.Models.EntityDto;
using ActivelyApp.Models.ServiceModels;
using ActivelyDomain.Entities;

namespace ActivelyApp.Services.EntityService
{
    public interface IPlayerService
    {
        public Task<ServiceResult<IEnumerable<Player>>> GetAllPlayers();
        public Task<ServiceResult<Player>> GetPlayerById(int id);
        public Task<ServiceResult<Player>> DeletePlayer(int id);
        public Task<ServiceResult<Player>> UpdatePlayer(Player player, int id);
        public Task<ServiceResult<Player>> CreatePlayer(Player newPlayer);
    }
}