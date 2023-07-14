using ActivelyDomain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly ActivelyDbContext _context;

        public PlayerRepository(ActivelyDbContext context)
        {
            _context = context;
        }

        public async Task Add(Player entity)
        {
            await _context.AddAsync(entity);
        }
        public async Task<IEnumerable<Player>> GetAll()
        {
            return await _context.Player
                 .Include(x => x.Games)
                 .ToListAsync();
        }
        public async Task Update(Player entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }
        public async Task<Player> GetById(int id)
        {
            return await _context.Player
                 .Include(x => x.Games)
                 .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task Remove(Player entity)
        {
            _context.Remove(entity);
        }

        public async Task Save() => await _context.SaveChangesAsync();
        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore(true).ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }
        protected virtual async ValueTask DisposeAsyncCore(bool disposing)
        {
            if (disposing)
                await _context.DisposeAsync().ConfigureAwait(false);
        }
    }
}
