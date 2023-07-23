using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;

namespace ActivelyApp.Services.EntityService
{
    public interface IGameService
    {
        Task Create(CreateGameInfo game);
        Task Delete(int id);
        Task<IEnumerable<Game>> GetAll();
        Task<Game> GetById(int id);
        Task Update(UpdateGameInfo game, int id);
    }
}