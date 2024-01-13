using ActivelyApp.CustomExceptions;
using ActivelyApp.Mappings;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using AutoMapper;
using Moq;
using Resources;
using Xunit;

namespace ActivelyApp.Tests.ServicesTests
{
    public class GameServiceTest
    {
        private IMapper _mapper;
        private Mock<IGameRepository> _gameRepository;
        private IGameService _gameService;
        private List<Game> _games;
        private Game _game;

        public GameServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(
               cfg => cfg.AddProfile<GameMappingProfile>());
            _gameRepository = new Mock<IGameRepository>();
            _gameService = new GameService(_gameRepository.Object);
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
        public async Task GetAll_ValidData_ReturnsNotNullCollectionAndCorrectGamesCount()
        {
            //arrange
            _gameRepository.Setup(g => g.GetAll()).ReturnsAsync(_games);

            //act
            var result = await _gameService.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, _games.Count);
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
            var result = await _gameService.GetById(id);
           
            //Assert
            if (isValid)
            {
                Assert.NotNull(result);
                Assert.IsType<GameDto>(result);                              
            }
            else
            {
                Assert.Null(result);
            }

        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(69, false)]
        public async Task Delete_ValidAndInvalidId_ReturnsExpectedResult(int id, bool isValid)
        {
            _gameRepository.Setup(repo => repo.GetById(id))
            .ReturnsAsync(isValid ? _game : null);
            //Arrange
            if (isValid)
            {
                _gameRepository.Setup(r => r.Delete(_game));
            }
            else
            {
                _gameRepository.Setup(r => r.Delete(It.IsAny<Game>())).ThrowsAsync(new NotFoundEntityException(Common.GameNotExistsError));               
            }
            //Act
            await _gameService.Delete(id);

            //Assert
            if (isValid)
            {              
                _gameRepository.Verify(repo => repo.Delete(_game), Times.Once);
                _gameRepository.Verify(repo => repo.Save(), Times.Once);
            }
            else
            {            
                _gameRepository.Verify(repo => repo.Delete(It.IsAny<Game>()), Times.Never);
                _gameRepository.Verify(repo => repo.Save(), Times.Never);
            }

        }

        [Fact]
        public async Task Update_ExistingGame_UpdatesGameTime()
        {
            // Arrange
            var gameToUpdate = new UpdateGameInfoDto() {

                GameTime = DateTime.Now.AddDays(25)       
            };

            
            _gameRepository.Setup(r => r.GetById(_game.Id)).ReturnsAsync(_game);


            // Act
            await _gameService.Update(gameToUpdate, _game.Id);

            // Assert

            Assert.Equal(gameToUpdate.GameTime, _game.GameTime);
            _gameRepository.Verify(repo => repo.Update(_game), Times.Once);
            _gameRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingGame_ThrowsNotFoundEntityException()
        {
            // Arrange
            var gameToUpdate = new UpdateGameInfoDto()
            {
                GameTime = DateTime.Now.AddDays(25)
            };

            _gameRepository.Setup(r => r.GetById(69)).ThrowsAsync(new NotFoundEntityException(Common.GameNotExistsError));


            // Act
            await _gameService.Update(gameToUpdate, 69);

            // Assert

            _gameRepository.Verify(repo => repo.Update(It.IsAny<Game>()), Times.Never);
            _gameRepository.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task Create_NullGameInfo_DoesNotCreateOrSaveGame()
        {
            // Arrange
            CreateGameInfoDto newGameInfo = null;

            // Act
            await _gameService.Create(newGameInfo);

            // Assert
            _gameRepository.Verify(repo => repo.Create(It.IsAny<Game>()), Times.Never);
            _gameRepository.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task Create_ValidGameInfo_CreatesAndSavesGame()
        {
            // Arrange
            CreateGameInfoDto newGameInfo = new CreateGameInfoDto()
            {
                Sport = SportType.HandBall
            };

            // Act
            await _gameService.Create(newGameInfo);

            // Assert
            _gameRepository.Verify(repo => repo.Create(It.IsAny<Game>()), Times.Once);
            _gameRepository.Verify(repo => repo.Save(), Times.Once);
        }
    }
}
