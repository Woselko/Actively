using ActivelyApp.CustomExceptions;
using ActivelyApp.Mappings;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.PlayerRepository;
using AutoMapper;
using Moq;
using Resources;
using Xunit;

namespace ActivelyApp.Tests.ServicesTests
{
    public class PlayerServiceTest
    {
        private IMapper _mapper;
        private Mock<IPlayerRepository> _playerRepository;
        private IPlayerService _playerService;
        private List<Player> _players;
        private Player _player;

        public PlayerServiceTest()
        {
            var mapperConfiguration = new MapperConfiguration(
               cfg => cfg.AddProfile<PlayerMappingProfile>());
            _mapper = new Mapper(mapperConfiguration);
            _playerRepository = new Mock<IPlayerRepository>();
            _playerService = new PlayerService(_playerRepository.Object, _mapper);
            _players = new List<Player>()
            {
                new Player{

                    Id = 999,
                    FirstName= "TestFirstName999",
                    LastName = "TesLastName999",
                    NickName = "TestNick999"

                    

                },
                new Player{

                    Id = 666,
                    FirstName= "TestFirstName666",
                    LastName = "TesLastName666",
                    NickName = "TestNick666"
                },

            };

            _player = new Player
            {

                Id = 10,
                FirstName = "TestFirstName10",
                LastName = "TesLastName10",
                NickName = "TestNick10"
            };
        }

        [Fact]
        public async Task GetAll_ValidData_ReturnsNotNullCollectionAndCorrectPlayersCount()
        {
            //arrange
            _playerRepository.Setup(p => p.GetAll()).ReturnsAsync(_players);

            //act
            var result = await _playerService.GetAll();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, _players.Count);
        }

        [Theory]
        [InlineData(10, true)]
        [InlineData(69, false)]
        public async Task GetById_ValidAndInvalidData_ReturnsExpectedResult(int id, bool isValid)
        {

            //Arrange
            if (isValid)
            {
                _playerRepository.Setup(r => r.GetById(id)).ReturnsAsync(_player);
            }
            else
            {
                if (id == 69)
                {

                    _playerRepository.Setup(repo => repo.GetById(id)).ReturnsAsync((Player)null);
                }

            }
            //Act
            var result = await _playerService.GetById(id);

            //Assert
            if (isValid)
            {
                Assert.NotNull(result);
                Assert.IsType<PlayerDto>(result);
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
            _playerRepository.Setup(repo => repo.GetById(id))
            .ReturnsAsync(isValid ? _player : null);
            //Arrange
            if (isValid)
            {
                _playerRepository.Setup(r => r.Delete(_player));
            }
            else
            {
                _playerRepository.Setup(r => r.Delete(It.IsAny<Player>())).ThrowsAsync(new NotFoundEntityException(Common.GameNotExistsError));
            }
            //Act
            await _playerService.Delete(id);

            //Assert
            if (isValid)
            {
                _playerRepository.Verify(repo => repo.Delete(_player), Times.Once);
                _playerRepository.Verify(repo => repo.Save(), Times.Once);
            }
            else
            {
                _playerRepository.Verify(repo => repo.Delete(It.IsAny<Player>()), Times.Never);
                _playerRepository.Verify(repo => repo.Save(), Times.Never);
            }

        }

        [Fact]
        public async Task Update_ExistingPlayer_UpdatesPlayerTime()
        {
            // Arrange
            var playerToUpdate = new UpdatePlayerInfo()
            {
               NickName = "newNickName"
            };


            _playerRepository.Setup(r => r.GetById(_player.Id)).ReturnsAsync(_player);


            // Act
            await _playerService.Update(playerToUpdate, _player.Id);

            // Assert

            Assert.Equal(playerToUpdate.NickName, _player.NickName);
            _playerRepository.Verify(repo => repo.Update(_player), Times.Once);
            _playerRepository.Verify(repo => repo.Save(), Times.Once);
        }

        [Fact]
        public async Task Update_NonExistingPlayer_ThrowsNotFoundEntityException()
        {
            // Arrange
            var playerToUpdate = new UpdatePlayerInfo()
            {
                NickName = "newNickName"
            };

            _playerRepository.Setup(r => r.GetById(69)).ThrowsAsync(new NotFoundEntityException(Common.GameNotExistsError));


            // Act
            await _playerService.Update(playerToUpdate, 69);

            // Assert

            _playerRepository.Verify(repo => repo.Update(It.IsAny<Player>()), Times.Never);
            _playerRepository.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task Create_NullPlayerInfo_DoesNotCreateOrSavePlayer()
        {
            // Arrange
            CreatePlayerInfo newPlayerInfo = null;

            // Act
            await _playerService.Create(newPlayerInfo);

            // Assert
            _playerRepository.Verify(repo => repo.Create(It.IsAny<Player>()), Times.Never);
            _playerRepository.Verify(repo => repo.Save(), Times.Never);
        }

        [Fact]
        public async Task Create_ValidPlayerInfo_CreatesAndSavesPlayer()
        {
            // Arrange
            CreatePlayerInfo newPlayerInfo = new CreatePlayerInfo()
            {
                NickName = "TestNewNickName",
                LastName = "TestNewLastName",
                FirstName = "TestNewName"
            };

            // Act
            await _playerService.Create(newPlayerInfo);

            // Assert
            _playerRepository.Verify(repo => repo.Create(It.IsAny<Player>()), Times.Once);
            _playerRepository.Verify(repo => repo.Save(), Times.Once);
        }
    }
}
