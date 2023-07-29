using ActivelyApp.Controllers;
using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActivelyApp.Tests.ControllersTests
{
    public class GameControllerTests
    {
        private Mock<IGameService> _mockGameService;
        private GameController _controller;
        private Game _game;
        private CreateGameInfo _newGame;
        private UpdateGameInfo _updatedGame;


        public GameControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            _controller = new GameController(_mockGameService.Object);
            _mockGameService.Setup(service => service.GetAll())
                 .ReturnsAsync(new List<GameDto>()
                 {
                    new GameDto(),
                    new GameDto()
                 });

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

            _newGame = new CreateGameInfo {  };
            _updatedGame = new UpdateGameInfo { };
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkObjectResultWithCorrectType()
        {
            //Arrange
            _mockGameService.Setup(service => service.GetAll())
                 .ReturnsAsync(new List<GameDto>()
                 {
                    new GameDto(),
                    new GameDto()
                 });
            // Act
            var actionResult = await _controller.GetAll();

            // Assert
            Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.IsAssignableFrom<IEnumerable<GameDto>>((actionResult.Result as OkObjectResult)?.Value);
        }

        [Fact]
        public async Task GetAll_ShouldReturnCorrectNumberOfGames()
        {
            //Arrange
            _mockGameService.Setup(service => service.GetAll())
                 .ReturnsAsync(new List<GameDto>()
                 {
                    new GameDto(),
                    new GameDto()
                 });
            // Act
            var actionResult = await _controller.GetAll();
            var games = (actionResult.Result as OkObjectResult)?.Value as List<GameDto>;

            // Assert
            Assert.NotNull(games);
            Assert.Equal(2, games.Count);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkObjectResultWithGameById()
        {
            //Arrange
            _mockGameService.Setup(service => service.GetById(_game.Id))
                 .ReturnsAsync(
                new GameDto()
                {
                    
                });
            // Act
            var actionResult = await _controller.GetById(10);
            var game = (actionResult as OkObjectResult)?.Value as GameDto;

            // Assert
            Assert.IsType<OkObjectResult>(actionResult);
            Assert.IsType<GameDto>(game);
        }

        [Fact]
        public async Task GetById_ShouldReturnAppropiateGame()
        {
            //Arrange
            _mockGameService.Setup(service => service.GetById(_game.Id))
                 .ReturnsAsync(
                new GameDto()
                {
                    GameTime = _game.GameTime,
                    CreationDate = _game.CreationDate,
                    Sport = _game.Sport

                });
            // Act
            var actionResult = await _controller.GetById(10);
            var game = (actionResult as OkObjectResult)?.Value as GameDto;

            // Assert
            Assert.NotNull(game);
            Assert.Equal(_game.GameTime, game.GameTime);
            Assert.Equal(_game.CreationDate, game.CreationDate);
            Assert.Equal(_game.Sport, game.Sport);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFound_WhenInvalidIdIsProvided()
        {
            // Arrange
            var invalidGameId = 69;

            // Act
            var actionResult = await _controller.GetById(invalidGameId);

            // Assert
            Assert.IsType<NotFoundResult>(actionResult);
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
                _mockGameService.Setup(service => service.Delete(id)).Verifiable();
            }
            else
            {
                if (id == 69)
                {
                    _mockGameService.Setup(service => service.Delete(id)).Throws(new NotFoundEntityException(Common.GameNotExistsError));
                }
                else
                {
                    _mockGameService.Setup(service => service.Delete(id)).Throws(new Exception(Common.SomethingWentWrong));
                }
            }

            // Act
            var result = await _controller.Delete(id);

            // Assert
            if (isValid)
            {
                Assert.IsType<OkObjectResult>(result);
                _mockGameService.Verify(service => service.Delete(10), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                    Assert.Equal(Common.GameNotExistsError, notFoundObjectResult.Value);
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
            _mockGameService.Setup(service => service.Create(_newGame)).Verifiable();

            // Act
            var result = await _controller.Create(_newGame);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(201, (result as StatusCodeResult)?.StatusCode);
            _mockGameService.Verify(service => service.Create(_newGame), Times.Once); // Ensure that the Create method on the service is called with the newGameInfo
        }

        [Fact]
        public async Task Create_WithNotFoundEntityException_ReturnsNotFoundResult()
        {
            // Arrange

            _mockGameService.Setup(service => service.Create(_newGame)).Throws(new NotFoundEntityException(Common.GameNotExistsError));

            // Act
            var result = await _controller.Create(_newGame);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(Common.SomethingWentWrong, badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Create_WithNullData_ReturnsBadRequestResult()
        {
            // Arrange
            CreateGameInfo newGameInfo = null;
            _mockGameService.Setup(service => service.Create(_newGame)).Verifiable();

            // Act
            var result = await _controller.Create(newGameInfo);

            // Assert
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(Common.SomethingWentWrong, badRequestObjectResult.Value);
        }

        [Fact]
        public async Task Update_WithNullData_ReturnsBadRequestResult()
        {
            int validGameId = 10;
            // Arrange
            UpdateGameInfo updateGameInfo = null;

            // Act
            var result = await _controller.Update(updateGameInfo, validGameId);

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
            var result = await _controller.Update(_updatedGame, id);

            // Assert
            if (isValid)
            {
                var okObjectResult = Assert.IsType<OkObjectResult>(result);
                Assert.Equal(Common.SuccessfullyUpdated, okObjectResult.Value);
                _mockGameService.Verify(service => service.Update(_updatedGame, id), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(result);
                    Assert.Equal(Common.GameNotExistsError, notFoundObjectResult.Value);
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
