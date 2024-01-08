using ActivelyApp.Controllers;
using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.Common;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Org.BouncyCastle.Crypto.Engines;
using Resources;
using Xunit;

namespace ActivelyApp.Tests.ControllersTests
{
    public class GameControllerTests
    {
        private Mock<IGameService> _mockGameService;
        private GameController _controller;
        private Game _game;
        private CreateGameInfoDto _newGame;
        private UpdateGameInfoDto _updatedGame;


        public GameControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            _controller = new GameController(_mockGameService.Object);   

            _game = new Game()
            {
                Id = 10,
                Sport = SportType.Football,
                CreationDate = DateTime.Now,
                GameTime= DateTime.Now,
                Players = new List<Player>()
                {
                    new Player(),
                    new Player()
                }
                
            };

            _newGame = new CreateGameInfoDto {  };
            _updatedGame = new UpdateGameInfoDto { };
        }

        [Fact]
        public async Task GetAll_ReturnsObjectResultSuccessfulWithData()
        {
            //Arrange
            _mockGameService.Setup(service => service.GetAll())
                 .ReturnsAsync(new List<GameDto>()
                 {
                    new GameDto(),
                    new GameDto()
                 });
            // Act
            var result = await _controller.GetAll() as ObjectResult;


            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.True(response.IsSuccess == true);
            Assert.Equal(Common.Success, response.Status);
        }
        

        [Fact]
        public async Task GetAll_ReturnsNotFoundResultWhenNoData()
        {
            //Arrange
             List<GameDto> games = null;
            _mockGameService.Setup(service => service.GetAll())
                 .ReturnsAsync(games);
            // Act
            var result = await _controller.GetAll() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.True(response.IsSuccess == false);
            Assert.Equal(Common.Error, response.Status);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequestResultOnError()
        {
            // Arrange
            string errorMessage = "test error message";
            _mockGameService.Setup(service => service.GetAll())
                 .ThrowsAsync(new Exception("test error message"));

            // Act

            var result = await _controller.GetAll() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(errorMessage, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.True(response.IsSuccess == false);
            Assert.Null(response.Content);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOkResultWithData()
        {
            //Arrange
            
            _mockGameService.Setup(service => service.GetById(_game.Id))
                 .ReturnsAsync(new GameDto() {CreationDate = _game.CreationDate, Sport = _game.Sport });
            // Act
            var result = await _controller.GetById(10) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.True(response.IsSuccess == true);
            Assert.Equal(Common.Success, response.Status);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task GetById_InvalidId_ShouldReturnNotFoundWithNoData()
        {
            // Arrange
            var invalidGameId = 69;

            // Act
            var result = await _controller.GetById(invalidGameId) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.True(response.IsSuccess == false);
            Assert.Equal(Common.Error, response.Status);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequestResultOnErrorWithNoData()
        {
            // Arrange
            string errorMessage = "test error message";
            _mockGameService.Setup(service => service.GetAll())
                 .ThrowsAsync(new Exception("test error message"));

            // Act

            var result = await _controller.GetAll() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(errorMessage, response.Message);
            Assert.True(response.IsSuccess == false);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.Content);
        }

        [Theory]
        [InlineData(10, true)] // Valid ID
        [InlineData(69, false)] // Invalid ID - Not Found Entity
        [InlineData(-1, false)] // Invalid ID - Bad Request
        public async Task Delete_WithValidAndInvalidId_ReturnsCorrectActionResult(int id, bool isValid)
        {
            // Arrange
            if (isValid)
            {
                _mockGameService.Setup(service => service.Delete(id)).Verifiable();
            }
            else
            {
                if (id == 69)
                {
                    _mockGameService.Setup(service => service.Delete(id)).ThrowsAsync(new NotFoundEntityException(Common.GameNotExistsError));
                }
                else
                {
                    _mockGameService.Setup(service => service.Delete(id)).ThrowsAsync(new Exception(Common.SomethingWentWrong));
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
                Assert.True(response.IsSuccess == true);
                Assert.Null(response.Content);
                _mockGameService.Verify(service => service.Delete(10), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.GameNotExistsError, response.Message);
                    Assert.Equal(Common.Error, response.Status);
                    Assert.True(response.IsSuccess == false);
                    Assert.Null(response.Content);
                }
                else
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.SomethingWentWrong, response.Message);
                    Assert.Equal(Common.Error, response.Status);
                    Assert.True(response.IsSuccess == false);
                    Assert.Null(response.Content);

                }
            }
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsObjectResultWithSuccessWithNoData()
        {
            //Arrange
            _mockGameService.Setup(service => service.Create(_newGame)).Verifiable();

            // Act
            var result = await _controller.Create(_newGame) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.Equal(Common.Success, response.Message);
            Assert.Equal(Common.Success, response.Status);
            Assert.True(response.IsSuccess == true);
            Assert.Null(response.Content);
            _mockGameService.Verify(service => service.Create(_newGame), Times.Once); 
        }

        [Fact]
        public async Task Create_ValidData_ReturnsBadRequestResultOnErrorWithNoData()
        {
            // Arrange
            _mockGameService.Setup(service => service.Create(_newGame))
                .ThrowsAsync(new Exception(Common.SomethingWentWrong)).Verifiable();

            // Act
            var result = await _controller.Create(_newGame) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.True(response.IsSuccess == false);
            Assert.Null(response.Content);
            _mockGameService.Verify(service => service.Create(_newGame), Times.Once);
        }

        [Fact]
        public async Task Create_NullData_ReturnsBadRequestResultWithNoData()
        {
            // Arrange
            CreateGameInfoDto newNullGameInfo = null;
            _mockGameService.Setup(service => service.Create(newNullGameInfo)).Verifiable();

            // Act
            var result = await _controller.Create(newNullGameInfo) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.Content);
            Assert.True(response.IsSuccess == false);
            _mockGameService.Verify(service => service.Create(newNullGameInfo), Times.Never);
        }

        [Fact]
        public async Task Update_NullData_ReturnsBadRequestResult()
        {
            int validGameId = 10;
            // Arrange
            UpdateGameInfoDto updateGameInfo = null;
            _mockGameService.Setup(service => service.Update(updateGameInfo, validGameId)).Verifiable();

            // Act
            var result = await _controller.Update(updateGameInfo, validGameId) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.True(response.IsSuccess == false);
            Assert.Null(response.Content);
            _mockGameService.Verify(service => service.Update(updateGameInfo, validGameId), Times.Never);
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
                _mockGameService.Setup(service => service.Delete(id)).Verifiable();
            }
            else
            {
                if (id == 69)
                {
                    _mockGameService.Setup(service => service.Update(_updatedGame, id)).Throws(new NotFoundEntityException(Common.GameNotExistsError));
                }
                else
                {
                    _mockGameService.Setup(service => service.Update(_updatedGame, id)).Throws(new Exception(Common.SomethingWentWrong));
                }
            }

            // Act
            var result = await _controller.Update(_updatedGame, id) as ObjectResult;

            // Assert
            if (isValid)
            {
                var response = Assert.IsType<Response>(result.Value);
                Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
                Assert.Equal(ResponseType.Success, response.Type);
                Assert.Equal(Common.SuccessfullyUpdated, response.Message);
                Assert.Equal(Common.Success, response.Status);
                Assert.Null(response.Content);
                Assert.True(response.IsSuccess == true);
                _mockGameService.Verify(service => service.Update(_updatedGame, id), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.GameNotExistsError, response.Message);
                    Assert.Equal(Common.Error, response.Status);
                    Assert.Null(response.Content);
                    Assert.True(response.IsSuccess == false);
                    _mockGameService.Verify(service => service.Update(_updatedGame, id), Times.Once);
                }
                else
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.SomethingWentWrong, response.Message);
                    Assert.Equal(Common.Error, response.Status);
                    Assert.True(response.IsSuccess == false);
                    Assert.Null(response.Content);
                    _mockGameService.Verify(service => service.Update(_updatedGame, id), Times.Once);
                }
            }
        }



    }
}
