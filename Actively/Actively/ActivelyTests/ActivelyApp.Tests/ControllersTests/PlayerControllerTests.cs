using ActivelyApp.Controllers;
using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.Common;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Resources;
using Xunit;

namespace ActivelyApp.Tests.ControllersTests
{
    public class PlayerControllerTests
    {
        private  Mock<IPlayerService> _mockPlayerService;
        private PlayerController _controller;
        private Player _player;
        private CreatePlayerInfoDto _newPlayer;
        private UpdatePlayerInfoDto _updatedPlayer;


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

            _newPlayer = new CreatePlayerInfoDto { FirstName = "firstName", LastName = "lastName", NickName = "nick" };
            _updatedPlayer = new UpdatePlayerInfoDto { LastName = "lastName", NickName = "nick" };
        }

        [Fact]
        public async Task GetAll_ReturnsObjectResultSuccessfulWithData()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.GetAll())
                 .ReturnsAsync(new List<PlayerDto>()
                 {
                    new PlayerDto(),
                    new PlayerDto()
                 });
            // Act
            var result = await _controller.GetAll() as ObjectResult;


            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.Equal(Common.Success, response.Status);
        }

        [Fact]
        public async Task GetAll_ReturnsNotFoundResultWhenNoData()
        {
            //Arrange
            List<PlayerDto> players = null;
            _mockPlayerService.Setup(service => service.GetAll())
                 .ReturnsAsync(players);
            // Act
            var result = await _controller.GetAll() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.Error, response.Status);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequestResultOnError()
        {
            // Arrange
            string errorMessage = "test error message";
            _mockPlayerService.Setup(service => service.GetAll())
                 .ThrowsAsync(new Exception("test error message"));

            // Act

            var result = await _controller.GetAll() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(errorMessage, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.ReturnObject);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOkResultWithData()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.GetById(_player.Id))
                 .ReturnsAsync(
                new PlayerDto()
                {
                    FirstName = _player.FirstName
                });
            // Act
            var result = await _controller.GetById(10) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.Equal(Common.Success, response.Status);
            Assert.NotNull(response.ReturnObject);
        }

        [Fact]
        public async Task GetById_InvalidId_ShouldReturnNotFoundWithNoData()
        {
            //Arrange
            var invalidPlayerId = 69;
            _mockPlayerService.Setup(service => service.GetById(_player.Id))
                 .ReturnsAsync(
                new PlayerDto()
                {
                    FirstName = _player.FirstName
                });
            //Act
            var result = await _controller.GetById(invalidPlayerId) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.Error, response.Status);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequestResultOnErrorWithNoData()
        {
            // Arrange
            string errorMessage = "test error message";
            _mockPlayerService.Setup(service => service.GetAll())
                 .ThrowsAsync(new Exception("test error message"));

            // Act

            var result = await _controller.GetAll() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(errorMessage, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.ReturnObject);
        }

        [Theory]
        [InlineData(10, true)] // Valid ID
        [InlineData(69, false)] // Invalid ID - Not Found Entity
        [InlineData(-1, false)] // Invalid ID - Bad Request
        public async Task Delete_ValidAndInvalidId_ReturnsCorrectActionResult(int id, bool isValid)
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
            var result = await _controller.Delete(id) as ObjectResult;

            // Assert
            if (isValid)
            {
                var response = Assert.IsType<Response>(result.Value);
                Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
                Assert.Equal(ResponseType.Success, response.Type);
                Assert.Equal(Common.SuccessfullyDeleted, response.Message);
                Assert.Equal(Common.Success, response.Status);
                Assert.Null(response.ReturnObject);
                _mockPlayerService.Verify(service => service.Delete(10), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.PlayerNotExistsError, response.Message);
                    Assert.Equal(Common.Error, response.Status);
                    Assert.Null(response.ReturnObject);
                }
                else
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.SomethingWentWrong, response.Message);
                    Assert.Equal(Common.Error, response.Status);
                    Assert.Null(response.ReturnObject);

                }
            }
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsObjectResultWithSuccessWithNoData()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.Create(_newPlayer)).Verifiable();

            // Act
            var result = await _controller.Create(_newPlayer) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.Equal(Common.Success, response.Message);
            Assert.Equal(Common.Success, response.Status);
            Assert.Null(response.ReturnObject);
            _mockPlayerService.Verify(service => service.Create(_newPlayer), Times.Once);
        }

        [Fact]
        public async Task Create_ValidData_ReturnsBadRequestResultOnErrorWithNoData()
        {
            // Arrange
            
            _mockPlayerService.Setup(service => service.Create(_newPlayer)).Throws(new NotFoundEntityException(Common.SomethingWentWrong));

            // Act
            var result = await _controller.Create(_newPlayer) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.ReturnObject);
            _mockPlayerService.Verify(service => service.Create(_newPlayer), Times.Once);
        }

        [Fact]
        public async Task Create_NullData_ReturnsBadRequestResultWithNoData()
        {
            // Arrange
            CreatePlayerInfoDto newPlayerInfo = null;
            _mockPlayerService.Setup(service => service.Create(_newPlayer)).Verifiable();

            // Act
            var result = await _controller.Create(newPlayerInfo) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.ReturnObject);
            _mockPlayerService.Verify(service => service.Create(_newPlayer), Times.Never);
        }

        [Fact]
        public async Task Update_NullData_ReturnsBadRequestResult()
        {
            int validPlayerId = 10;
            // Arrange
            UpdatePlayerInfoDto updatePlayerInfo = null;

            // Act
            var result = await _controller.Update(updatePlayerInfo, validPlayerId) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.ReturnObject);
            _mockPlayerService.Verify(service => service.Update(updatePlayerInfo, validPlayerId), Times.Never);
        }

        [Theory]
        [InlineData(10, true)] // Valid ID
        [InlineData(69, false)] // Invalid ID - Not Found Entity
        [InlineData(-1, false)] // Invalid ID - Bad Request
        public async Task Update_ValidOrInvalidId_ReturnsCorrectActionResult(int id, bool isValid)
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
            var result = await _controller.Update(_updatedPlayer, id) as ObjectResult;

            // Assert
            if (isValid)
            {
                var response = Assert.IsType<Response>(result.Value);
                Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
                Assert.Equal(ResponseType.Success, response.Type);
                Assert.Equal(Common.SuccessfullyUpdated, response.Message);
                Assert.Equal(Common.Success, response.Status);
                Assert.Null(response.ReturnObject);
                _mockPlayerService.Verify(service => service.Update(_updatedPlayer, id), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.PlayerNotExistsError, response.Message);
                    Assert.Equal(Common.Error, response.Status);
                    Assert.Null(response.ReturnObject);
                    _mockPlayerService.Verify(service => service.Update(_updatedPlayer, id), Times.Once);
                }
                else
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.SomethingWentWrong, response.Message);
                    Assert.Equal(Common.Error, response.Status);
                    Assert.Null(response.ReturnObject);
                    _mockPlayerService.Verify(service => service.Update(_updatedPlayer, id), Times.Once);
                }
            }
        }



    }
}
