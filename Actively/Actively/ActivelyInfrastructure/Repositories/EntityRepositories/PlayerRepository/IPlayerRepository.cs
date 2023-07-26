using ActivelyDomain.Entities;

namespace ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository
{
    public interface IPlayerRepository : IAsyncDisposable
    {
        Task Create(Player entity);
        Task<IEnumerable<Player>> GetAll();
        Task<Player> GetById(int id);
        Task Delete(Player entity);
        Task Save();
        Task Update(Player entity);
    }
}