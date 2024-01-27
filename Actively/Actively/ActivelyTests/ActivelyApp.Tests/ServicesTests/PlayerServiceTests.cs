using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Resources;
using Xunit;

namespace ActivelyApp.Tests.ServicesTests
{
    public class PlayerServiceTest
    {
        private IMapper _mapper;
        private Mock<IPlayerRepository> _playerRepository;
        private Mock<ILogger<PlayerService>> _logger;
        private IPlayerService _playerService;
        private List<Player> _players;
        private Player _player01;
        private Player _player02;

        public PlayerServiceTest()
        {
            _playerRepository = new Mock<IPlayerRepository>();
            _logger = new Mock<ILogger<PlayerService>>();
            _playerService = new PlayerService(_playerRepository.Object, _logger.Object);
            _players = new List<Player>()
            {
                new Player{

                    Id = 999,
                    FirstName= "Test1",
                    LastName= "Test1",
                    NickName= "Test1",

                },
                new Player{

                    Id = 666,
                    FirstName= "Test1",
                    LastName= "Test1",
                    NickName= "Test1",
                },

            };

            _player01 = new Player
            {

                Id = 10,
                FirstName = "Test1",
                LastName = "Test1",
                NickName = "Test1",
            };

            _player02 = new Player
            {

                Id = 20,
                FirstName = "Test2",
                LastName = "Test2",
                NickName = "Test2",
            };
        }

        [Fact]
        public async Task GetAll_ValidData_ReturnsPlayersCollectionSuccessfully()
        {
            //arrange
            _playerRepository.Setup(g => g.GetAll()).ReturnsAsync(_players);

            //act
            var result = await _playerService.GetAllPlayers();

            //Assert
            Assert.Equal(2, result.Data.Count());
            Assert.Equal(Common.Success, result.Message);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task GetAll_ValidData_ReturnsEmptyCollectionSuccessfully()
        {
            //arrange
            _playerRepository.Setup(g => g.GetAll()).ReturnsAsync(new List<Player>());

            //act
            var result = await _playerService.GetAllPlayers();

            //Assert
            Assert.Empty(result.Data);
            Assert.Equal(Common.PlayerNotExists, result.Message);
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
                _playerRepository.Setup(r => r.GetById(id)).ReturnsAsync(_player01);
            }
            else
            {
                if (id == 69)
                {
                    _playerRepository.Setup(repo => repo.GetById(id)).ReturnsAsync((Player)null);
                }

            }
            //Act
            var result = await _playerService.GetPlayerById(id);

            //Assert
            if (isValid)
            {
                Assert.NotNull(result.Data);
                Assert.Equal(Common.Success, result.Message);
                Assert.True(result.IsSuccess);
            }
            else if (!isValid && id == 69)
            {
                Assert.Equal(Common.PlayerNotExists, result.Message);
                Assert.False(result.IsSuccess);
            }
        }

        [Fact]
        public async Task GetPlayerById_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var playerId = 1;

            _playerRepository.Setup(repo => repo.GetById(playerId)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _playerService.GetPlayerById(playerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Common.SomethingWentWrong, result.Message);
        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(69, false)]
        public async Task Delete_ValidAndInvalidPlayer_ReturnsExpectedResult(int id, bool isValid)
        {
            _playerRepository.Setup(repo => repo.GetById(id))
            .ReturnsAsync(isValid ? _player01 : null);
            //Arrange
            if (isValid)
            {
                _playerRepository.Setup(r => r.Delete(_player01));
            }
            else
            {
                _playerRepository.Setup(repo => repo.GetById(id)).ReturnsAsync((Player)null);
            }
            //Act
            var result = await _playerService.DeletePlayer(id);

            //Assert
            if (isValid)
            {
                _playerRepository.Verify(repo => repo.Delete(_player01), Times.Once);
                _playerRepository.Verify(repo => repo.Save(), Times.Once);
                Assert.Equal(Common.SuccessfullyDeleted, result.Message);
                Assert.True(result.IsSuccess);
            }
            else
            {
                _playerRepository.Verify(repo => repo.Delete(It.IsAny<Player>()), Times.Never);
                _playerRepository.Verify(repo => repo.Save(), Times.Never);
                Assert.Equal(Common.PlayerNotExists, result.Message);
                Assert.False(result.IsSuccess);
            }
        }

        [Fact]
        public async Task DeletePlayer_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var playerId = 1;

            _playerRepository.Setup(repo => repo.GetById(playerId)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _playerService.DeletePlayer(playerId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Common.SomethingWentWrong, result.Message);
            _playerRepository.Verify(repo => repo.Delete(It.IsAny<Player>()), Times.Never);
            _playerRepository.Verify(repo => repo.Save(), Times.Never);
        }


        [Fact]
        public async Task Update_ExistingPlayer_ReturnCorrectResultandUpdatesTime()
        {
            // Arrange
            var updatePlayerInfo = new Player()
            {
                FirstName = "Test",
                LastName = "Test",
                NickName = "Test",
            };

            _playerRepository.Setup(r => r.GetById(_player01.Id)).ReturnsAsync(_player01);

            // Act
            var result = await _playerService.UpdatePlayer(updatePlayerInfo, _player01.Id);

            // Assert

            Assert.Equal(Common.SuccessfullyUpdated, result.Message);
            Assert.True(result.IsSuccess);
            Assert.Equal(_player01.LastName, _player01.LastName);
            _playerRepository.Verify(repo => repo.Update(_player01), Times.Once);
            _playerRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingPlayer_ReturnCorrectResultandDoNotUpdatesTime()
        {
            // Arrange

            _playerRepository.Setup(repo => repo.GetById(_player01.Id)).ReturnsAsync((Player)null);

            // Act
            var result = await _playerService.UpdatePlayer(_player01, _player01.Id);

            // Assert
            Assert.Equal(Common.PlayerNotExists, result.Message);
            Assert.False(result.IsSuccess);
            _playerRepository.Verify(repo => repo.Update(It.IsAny<Player>()), Times.Never);
            _playerRepository.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task UpdatePlayer_ExceptionOccurs_ReturnsFailure()
        {
            // Arrange

            _playerRepository.Setup(repo => repo.GetById(_player01.Id)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _playerService.UpdatePlayer(_player02, _player01.Id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Common.SomethingWentWrong, result.Message);
            _playerRepository.Verify(repo => repo.Delete(It.IsAny<Player>()), Times.Never);
            _playerRepository.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task CreatePlayer_ValidInput_CreatesAndReturnsSuccess()
        {
            // Arrange

            // Act
            var result = await _playerService.CreatePlayer(_player01);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(Common.SuccessfullyCreated, result.Message);

            _playerRepository.Verify(repo => repo.Create(_player01), Times.Once);
            _playerRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task CreatePlayer_ExceptionOccurs_ReturnsFailure()
        {
            _playerRepository.Setup(repo => repo.Create(_player01)).ThrowsAsync(new Exception("Some error"));
            // Act
            var result = await _playerService.CreatePlayer(_player01);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(Common.SomethingWentWrong, result.Message);

            _playerRepository.Verify(repo => repo.Save(), Times.Never);
        }
    }
}
