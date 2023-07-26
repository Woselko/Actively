using ActivelyApp.Controllers;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using ActivelyApp.CustomExceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Resources;
using Xunit;
using System.Numerics;

namespace ActivelyApp.Tests.ControllersTests
{
    public class PlayerControllerTests
    {
        private  Mock<IPlayerService> _mockPlayerService;
        private PlayerController _controller;
        private Player _player;
        private CreatePlayerInfo _newPlayer;
        private UpdatePlayerInfo _updatedPlayer;


        public PlayerControllerTests()
        {
            _mockPlayerService= new Mock<IPlayerService>();
            _controller = new PlayerController(_mockPlayerService.Object);
            _mockPlayerService.Setup(service => service.GetAll())
                 .ReturnsAsync(new List<PlayerDto>()
                 {
                    new PlayerDto(),
                    new PlayerDto()
                 });

            _player = new Player()
            {
                Id = 10,
                FirstName = "TestName",
                NickName = "TestNick"
            };

            _newPlayer = new CreatePlayerInfo { FirstName = "firstName", LastName = "lastName", NickName = "nick" };
            _updatedPlayer = new UpdatePlayerInfo { LastName = "lastName", NickName = "nick" };
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkObjectResultWithCorrectType()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.GetAll())
                 .ReturnsAsync(new List<PlayerDto>()
                 {
                    new PlayerDto(),
                    new PlayerDto()
                 });
            // Act
            var actionResult = await _controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.IsAssignableFrom<IEnumerable<PlayerDto>>((actionResult.Result as OkObjectResult)?.Value);
        }

        [Fact]
        public async Task GetAll_ShouldReturnCorrectNumberOfPlayers()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.GetAll())
                 .ReturnsAsync(new List<PlayerDto>()
                 {
                    new PlayerDto(),
                    new PlayerDto()
                 });
            // Act
            var actionResult = await _controller.GetAll();
            var players = (actionResult.Result as OkObjectResult)?.Value as List<PlayerDto>;

            // Assert
            Assert.NotNull(players);
            Assert.Equal(2, players.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkObjectResultWithPlayerById()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.GetById(_player.Id))
                 .ReturnsAsync(
                new PlayerDto()
                {
                    FirstName = _player.FirstName
                });
            // Act
            var actionResult = await _controller.GetById(10);
            var player = (actionResult.Result as OkObjectResult)?.Value as PlayerDto;

            // Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.IsType<PlayerDto>(player);
        }

        [Fact]
        public async Task GetById_ShouldReturnAppropiatePlayer()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.GetById(_player.Id))
                 .ReturnsAsync(
                new PlayerDto()
                {
                    FirstName = _player.FirstName
                });
            // Act
            var actionResult = await _controller.GetById(10);
            var player = (actionResult.Result as OkObjectResult)?.Value as PlayerDto;

            // Assert
            Assert.NotNull(player);
            Assert.Equal("TestName", player.FirstName);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenInvalidIdIsProvided()
        {
            // Arrange
            var invalidPlayerId = 69;

            // Act
            var actionResult = await _controller.GetById(invalidPlayerId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult.Result);
        }

        [Theory]
        [InlineData(10, true)] // Valid ID
        [InlineData(69, false)] // Invalid ID - Not Found Entity
        [InlineData(-1, false)] // Invalid ID - Bad Request
        public async Task Delete_WithValidOrInvalidId_ReturnsCorrectActionResult(int id, bool isValid)
        {
            // Arrange
            if (isValid)
            {       
                _mockPlayerService.Setup(service => service.Delete(id)).Verifiable();
            }
            else
            {
                if (id == 69)
                {
                    _mockPlayerService.Setup(service => service.Delete(id)).Throws(new NotFoundEntityException(Common.PlayerNotExistsError));
                }
                else
                {
                    _mockPlayerService.Setup(service => service.Delete(id)).Throws(new Exception(Common.SomethingWentWrong));
                }
            }

            // Act
            var result = await _controller.Delete(id);

            // Assert
            if (isValid)
            {
                Assert.IsType<OkObjectResult>(result);
                _mockPlayerService.Verify(service => service.Delete(10), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                    Assert.Equal(Common.PlayerNotExistsError, notFoundObjectResult.Value);
                }
                else
                {
                    var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                    Assert.Equal(Common.SomethingWentWrong, badRequestObjectResult.Value);
                }
            }
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedResult()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.Create(_newPlayer)).Verifiable();

            // Act
            var result = await _controller.Create(_newPlayer);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, (result as StatusCodeResult)?.StatusCode);
            _mockPlayerService.Verify(service => service.Create(_newPlayer), Times.Once); // Ensure that the Create method on the service is called with the newPlayerInfo
        }

        [Fact]
        public async Task Create_WithNotFoundEntityException_ReturnsNotFoundResult()
        {
            // Arrange
            
            _mockPlayerService.Setup(service => service.Create(_newPlayer)).Throws(new NotFoundEntityException(Common.PlayerNotExistsError));

            // Act
            var result = await _controller.Create(_newPlayer);

            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(Common.PlayerNotExistsError, notFoundObjectResult.Value);
        }

        [Fact]
        public async Task Create_WithNullData_ReturnsBadRequestResult()
        {
            // Arrange
            CreatePlayerInfo newPlayerInfo = null;
            _mockPlayerService.Setup(service => service.Create(_newPlayer)).Verifiable();

            // Act
            var result = await _controller.Create(newPlayerInfo);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(Common.SomethingWentWrong, badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Update_WithNullData_ReturnsBadRequestResult()
        {
            int validPlayerId = 10;
            // Arrange
            UpdatePlayerInfo updatePlayerInfo = null;

            // Act
            var result = await _controller.Update(updatePlayerInfo, validPlayerId);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(Common.SomethingWentWrong, badRequestObjectResult.Value);
        }

        [Theory]
        [InlineData(10, true)] // Valid ID
        [InlineData(69, false)] // Invalid ID - Not Found Entity
        [InlineData(-1, false)] // Invalid ID - Bad Request
        public async Task Update_WithValidOrInvalidId_ReturnsCorrectActionResult(int id, bool isValid)
        {

            // Arrange
            if (isValid)
            {
                _mockPlayerService.Setup(service => service.Delete(id)).Verifiable();
            }
            else
            {
                if (id == 69)
                {
                    _mockPlayerService.Setup(service => service.Update(_updatedPlayer, id)).Throws(new NotFoundEntityException(Common.PlayerNotExistsError));
                }
                else
                {
                    _mockPlayerService.Setup(service => service.Update(_updatedPlayer, id)).Throws(new Exception(Common.SomethingWentWrong));
                }
            }

            // Act
            var result = await _controller.Update(_updatedPlayer, id);

            // Assert
            if (isValid)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(Common.SuccessfullyUpdated, okObjectResult.Value);
                _mockPlayerService.Verify(service => service.Update(_updatedPlayer, id), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                    Assert.Equal(Common.PlayerNotExistsError, notFoundObjectResult.Value);
                }
                else
                {
                    var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
                    Assert.Equal(Common.SomethingWentWrong, badRequestObjectResult.Value);
                }
            }
        }



    }
}
