using ActivelyApp.Models.EntityDto;
using ActivelyDomain.Entities;

namespace ActivelyApp.Services.EntityService
{
    public interface IGameService
    {
        Task Create(CreateGameInfo game);
        Task Delete(int id);
        Task<IEnumerable<GameDto>> GetAll();
        Task<GameDto> GetById(int id);
        Task Update(UpdateGameInfo game, int id);
    }
}