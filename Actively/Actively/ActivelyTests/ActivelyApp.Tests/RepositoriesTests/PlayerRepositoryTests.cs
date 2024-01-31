using ActivelyDomain.Entities;
using ActivelyInfrastructure;
using ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ActivelyApp.Tests.RepositoriesTests
{
    public class PlayerRepositoryTests : IDisposable
    {
        private readonly ActivelyDbContext _dbContext;
        private PlayerRepository _repository;
        private Player _player;
        private List<Player> _players;
        public PlayerRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ActivelyDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new ActivelyDbContext(options);
            _dbContext.Database.EnsureCreated();
            _repository = new PlayerRepository(_dbContext);
            _player = new Player()
            {
                Id = 999,
                FirstName = "TestName",
                LastName = "TestLastName",
                NickName = "TestNick"
            };
            _players = new List<Player>() {
                _player,
                new Player()
                {
                Id = 666,
                FirstName = "TestName2",
                LastName = "TestLastName2",
                NickName = "TestNick2"
                }
            };
        }

        [Fact]
        public async Task Add_ValidPlayer_ShouldAddPlayerToDatabase()
        {
            // Act
            await _repository.Create(_player);
            await _repository.Save();

            // Assert
            var result = await _dbContext.Player.FirstOrDefaultAsync(p => p.Id == _player.Id);
            Assert.NotNull(result);
            Assert.Equal(_player.Id, result.Id);
        }

        [Fact]
        public async Task Remove_ValidPlayer_ShouldRemoveEntityFromContext()
        {
            // Arrange
            await _repository.Create(_player);
            await _repository.Save();

            // Act
            await _repository.Delete(_player);
            await _repository.Save();

            // Assert
            var result = await _dbContext.Player.SingleOrDefaultAsync(p => p.Id == _player.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAll_ValidGames_ShouldReturnMatchingPlayers()
        {
            //Arrange
            await _dbContext.AddAsync(_players[0]);
            await _dbContext.AddAsync(_players[1]);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task Update_ValidPlayer_ShouldUpdatePlayerInDatabase()
        {
            // Arrange
            await _dbContext.AddAsync(_player);
            await _dbContext.SaveChangesAsync();
            _player.FirstName = "TestUpdate";

            // Act
            await _repository.Update(_player);

            // Assert
            var result = await _dbContext.Player.FirstOrDefaultAsync(p => p.Id == _player.Id);
            Assert.NotNull(result);
            Assert.Equal(_player.FirstName, result.FirstName);
        }

        [Fact]
        public async Task GetById_ValidPlayer_ShouldGetPlayerWithProvidedId()
        {
            //Arrange
            await _dbContext.AddAsync(_player);
            await _dbContext.SaveChangesAsync();

            //act
            var result = await _repository.GetById(_player.Id);

            //assert
            Assert.NotNull(result);
            Assert.Equal(_player.Id, result.Id);
        }

        [Fact]
        public async Task GetById_NonExistingPlayer_ShouldReturnNull()
        {
            // Arrange
            int invalidId = 9999;
            await _dbContext.AddAsync(_player);

            // Act
            var result = await _repository.GetById(invalidId);

            // Assert
            Assert.Null(result);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
