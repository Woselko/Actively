using ActivelyApp.Models.EntityDto;
using ActivelyApp.Models.ServiceModels;
using ActivelyDomain.Entities;

namespace ActivelyApp.Services.EntityService
{
    public interface IGameService
    {
        public Task<ServiceResult<IEnumerable<Game>>> GetAllGames();
        public Task<ServiceResult<Game>> GetGameById(int id);
        public Task<ServiceResult<Game>> DeleteGame(int id);
        public Task<ServiceResult<Game>> UpdateGame(Game game, int id);
        public Task<ServiceResult<Game>> CreateGame(Game newGame);
    }
}