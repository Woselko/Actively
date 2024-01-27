using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using Microsoft.Extensions.Logging;
using Moq;
using Resources;
using Xunit;

namespace ActivelyApp.Tests.ServicesTests
{
    public class GameServiceTest
    {
        private Mock<IGameRepository> _gameRepository;
        private Mock<ILogger<GameService>> _logger;
        private IGameService _gameService;
        private List<Game> _games;
        private Game _game;
        public GameServiceTest()
        {
            _gameRepository = new Mock<IGameRepository>();
            _logger = new Mock<ILogger<GameService>>();
            _gameService = new GameService(_gameRepository.Object, _logger.Object);
            _games = new List<Game>()
            {
                new Game{
                    Id = 999,
                    Players = new List<Player>(),
                    CreationDate = DateTime.Now,
                    GameTime= DateTime.Now,
                    Sport = SportType.Football
                },
                new Game{
                    Id = 666,
                    Players = new List<Player>(),
                    CreationDate = DateTime.Now,
                    GameTime= DateTime.Now,
                    Sport = SportType.Football
                },
            };
            _game = new Game
            {
                Id = 10,
                Players = new List<Player>(),
                CreationDate = DateTime.Now.AddDays(55),
                GameTime = DateTime.Now.AddDays(55),
                Sport = SportType.Football
            };
        }

        [Fact]
        public async Task GetAll_ValidData_ReturnsGamesCollectionSuccessfully()
        {
            //arrange
            _gameRepository.Setup(g => g.GetAll()).ReturnsAsync(_games);

            //act
            var result = await _gameService.GetAllGames();

            //Assert
            Assert.Equal(2, result.Data.Count());
            Assert.Equal(Common.Success, result.Message);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAll_ValidData_ReturnsEmptyCollectionSuccessfully()
        {
            //arrange
            _gameRepository.Setup(g => g.GetAll()).ReturnsAsync(new List<Game>());

            //act
            var result = await _gameService.GetAllGames();

            //Assert
            Assert.Empty(result.Data);
            Assert.Equal(Common.GameNotExists, result.Message);
            Assert.True(result.IsSuccess);
        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(69, false)]
        public async Task GetById_ValidAndInvalidData_ReturnsExpectedResult(int id, bool isValid)
        {
            //Arrange
            if (isValid)
            {
                _gameRepository.Setup(r => r.GetById(id)).ReturnsAsync(_game);
            }
            else
            {
                if (id == 69)
                {
                    _gameRepository.Setup(repo => repo.GetById(id)).ReturnsAsync((Game)null);
                }
            }
            //Act
            var result = await _gameService.GetGameById(id);

            //Assert
            if (isValid)
            {
                Assert.NotNull(result.Data);
                Assert.Equal(Common.Success, result.Message);
                Assert.True(result.IsSuccess);
            }
            else if (!isValid && id == 69)
            {
                Assert.Equal(Common.GameNotExists, result.Message);
                Assert.False(result.IsSuccess);
            }
        }

        [Fact]
        public async Task GetGameById_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var gameId = 1;
            _gameRepository.Setup(repo => repo.GetById(gameId)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _gameService.GetGameById(gameId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Common.SomethingWentWrong, result.Message);
        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(69, false)]
        public async Task Delete_ValidAndInvalidGame_ReturnsExpectedResult(int id, bool isValid)
        {
			//Arrange
			_gameRepository.Setup(repo => repo.GetById(id))
            .ReturnsAsync(isValid ? _game : null);
			if (isValid)
            {
                _gameRepository.Setup(r => r.Delete(_game));
            }
            else
            {
                _gameRepository.Setup(repo => repo.GetById(id)).ReturnsAsync((Game)null);
            }
            //Act
            var result = await _gameService.DeleteGame(id);

            //Assert
            if (isValid)
            {
                _gameRepository.Verify(repo => repo.Delete(_game), Times.Once);
                _gameRepository.Verify(repo => repo.Save(), Times.Once);
                Assert.Equal(Common.SuccessfullyDeleted, result.Message);
                Assert.True(result.IsSuccess);
            }
            else
            {
                _gameRepository.Verify(repo => repo.Delete(It.IsAny<Game>()), Times.Never);
                _gameRepository.Verify(repo => repo.Save(), Times.Never);
                Assert.Equal(Common.GameNotExists, result.Message);
                Assert.False(result.IsSuccess);
            }
        }

        [Fact]
        public async Task DeleteGame_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var gameId = 1;
            _gameRepository.Setup(repo => repo.GetById(gameId)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _gameService.DeleteGame(gameId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Common.SomethingWentWrong, result.Message);
            _gameRepository.Verify(repo => repo.Delete(It.IsAny<Game>()), Times.Never);
            _gameRepository.Verify(repo => repo.Save(), Times.Never);
        }


        [Fact]
        public async Task Update_ExistingGame_ReturnCorrectResultandUpdatesTime()
        {
            // Arrange
            var gameToUpdate = new Game
            {
                GameTime = DateTime.Now.AddDays(25)
            };
            _gameRepository.Setup(r => r.GetById(_game.Id)).ReturnsAsync(_game);

            // Act
            var result = await _gameService.UpdateGame(gameToUpdate, _game.Id);

            // Assert
            Assert.Equal(Common.SuccessfullyUpdated, result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(gameToUpdate.GameTime, _game.GameTime);
            _gameRepository.Verify(repo => repo.Update(_game), Times.Once);
            _gameRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingGame_ReturnCorrectResultandDoNotUpdatesTime()
        {
            // Arrange
            var gameToUpdate = new Game()
            {
                Id = 15,
                GameTime = DateTime.Now.AddDays(25)
            };

            _gameRepository.Setup(repo => repo.GetById(gameToUpdate.Id)).ReturnsAsync((Game)null);

            // Act
            var result = await _gameService.UpdateGame(gameToUpdate, 69);

            // Assert
            Assert.Equal(Common.GameNotExists, result.Message);
            Assert.False(result.IsSuccess);
            _gameRepository.Verify(repo => repo.Update(It.IsAny<Game>()), Times.Never);
            _gameRepository.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task UpdateGame_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var gameToUpdate = new Game()
            {
                Id = 15,
                GameTime = DateTime.Now.AddDays(25)
            };

            _gameRepository.Setup(repo => repo.GetById(gameToUpdate.Id)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _gameService.UpdateGame(gameToUpdate, gameToUpdate.Id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Common.SomethingWentWrong, result.Message);
            _gameRepository.Verify(repo => repo.Delete(It.IsAny<Game>()), Times.Never);
            _gameRepository.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task CreateGame_ValidInput_CreatesAndReturnsSuccess()
        {
            // Arrange
            var newGame = new Game
            { 
                Id = 15,
                CreationDate = DateTime.Today,
                GameTime = DateTime.Today,
                Sport = SportType.BasketBall,
                Players = new List<Player>()
            };

            // Act
            var result = await _gameService.CreateGame(newGame);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Common.SuccessfullyCreated, result.Message);

            _gameRepository.Verify(repo => repo.Create(newGame), Times.Once);
            _gameRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task CreateGame_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var newGame = new Game
            {
                Id = 15,
                CreationDate = DateTime.Today,
                GameTime = DateTime.Today,
                Sport = SportType.BasketBall,
                Players = new List<Player>()
            };
            _gameRepository.Setup(repo => repo.Create(newGame)).ThrowsAsync(new Exception("Some error"));
            // Act
            var result = await _gameService.CreateGame(newGame);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Common.SomethingWentWrong, result.Message);

            _gameRepository.Verify(repo => repo.Save(), Times.Never);
        }
    }
}