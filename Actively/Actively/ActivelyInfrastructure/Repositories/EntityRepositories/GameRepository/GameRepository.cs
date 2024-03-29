﻿using ActivelyDomain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository
{
    public class GameRepository : IGameRepository
    {
        private readonly ActivelyDbContext _context;

        public GameRepository(ActivelyDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Game>> GetAll()
        {
            return await _context.Game.ToListAsync();
        }

        public async Task<Game> GetById(int id)
        {
            return await _context.Game
                .Include(x => x.Players)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task Delete(Game game)
        { 
            _context.Game.Remove(game);
        }

        public async Task Update(Game entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public async Task Create(Game entity)
        {
            await _context.AddAsync(entity);
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
