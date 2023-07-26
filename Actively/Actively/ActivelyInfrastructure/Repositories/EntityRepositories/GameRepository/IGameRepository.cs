using ActivelyDomain.Entities;

namespace ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository
{
    public interface IGameRepository : IAsyncDisposable
    {
        Task Create(Game game);
        Task Delete(int id);
        Task<IEnumerable<Game>> GetAll();
        Task<Game> GetById(int id);
        Task Save();
        Task Update(Game game);
    }
}