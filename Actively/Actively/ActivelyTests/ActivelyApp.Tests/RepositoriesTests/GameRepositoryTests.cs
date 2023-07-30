using ActivelyDomain.Entities;
using ActivelyInfrastructure;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ActivelyApp.Tests.RepositoriesTests
{

    public class GameRepositoryTests : IDisposable
        {
            private readonly ActivelyDbContext _dbContext;
            private GameRepository _repository;
            private Game _game;
            private List<Game> _games;

            public GameRepositoryTests()
            {
                var options = new DbContextOptionsBuilder<ActivelyDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options;

                _dbContext = new ActivelyDbContext(options);
                _dbContext.Database.EnsureCreated();
                _repository = new GameRepository(_dbContext);

                _game = new Game()
                {
                    Id = 999,
                    CreationDate = DateTime.Now,
                    GameTime = DateTime.Now,
                    Sport = SportType.Football,
                    Players= new List<Player>() { }
                };

                _games = new List<Game>() {

                _game,

                new Game()
                {
                    Id = 666,
                    CreationDate = DateTime.Now,
                    GameTime = DateTime.Now,
                    Sport = SportType.Volleyball,
                }
            };

            }

            [Fact]
            public async Task Add_ValidGame_ShouldAddGameToDbContext()
            {

                // Act
                await _repository.Create(_game);
                await _repository.Save();

                // Assert
                var result = await _dbContext.Game.FirstOrDefaultAsync(p => p.Id == _game.Id);
                Assert.NotNull(result);
                Assert.Equal(_game.Id, result.Id);
            }

            [Fact]
            public async Task Remove_ValidGame_ShouldRemoveGameFromDbContext()
            {
                // Arrange
                await _repository.Create(_game);
                await _repository.Save();


                // Act
                await _repository.Delete(_game);
                await _repository.Save();

                // Assert

                var result = await _dbContext.Game.SingleOrDefaultAsync(p => p.Id == _game.Id);
                Assert.Null(result);
            }

            [Fact]
            public async Task GetAll_ValidGames_ShouldReturnMatchingGamesFromDbContext()
            {
                //Arrange
                await _dbContext.AddAsync(_games[0]);
                await _dbContext.AddAsync(_games[1]);
                await _dbContext.SaveChangesAsync();

                // Act
                var result = await _repository.GetAll();

                // Assert
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }


            [Fact]
            public async Task Update_ValidGame_ShouldUpdateGameInDbContext()
            {
                // Arrange
                await _dbContext.AddAsync(_game);
                await _dbContext.SaveChangesAsync();

                _game.Sport = SportType.HandBall;

                // Act
                await _repository.Update(_game);

                // Assert
                var result = await _dbContext.Game.FirstOrDefaultAsync(p => p.Id == _game.Id);
                Assert.NotNull(result);
                Assert.Equal(_game.Sport, result.Sport);

            }

            [Fact]
            public async Task GetById_ExistingGame_ShouldGetGameWithProvidedIdFromDbContext()
            {
                //Arrange
                await _dbContext.AddAsync(_game);
                await _dbContext.SaveChangesAsync();

                //act
                var result = await _repository.GetById(_game.Id);

                //assert
                Assert.NotNull(result);
                Assert.Equal(_game.Id, result.Id);
            }

            [Fact]
            public async Task GetById_NonExistingGame_ShouldReturnNull()
            {
                // Arrange
                int invalidId = 9999;
                await _dbContext.AddAsync(_game);

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
