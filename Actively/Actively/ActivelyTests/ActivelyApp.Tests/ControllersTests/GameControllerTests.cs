using ActivelyApp.Controllers;
using ActivelyApp.Mappings;
using ActivelyApp.Models.Common;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Models.ServiceModels;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Resources;
using Xunit;

namespace ActivelyApp.Tests.ControllersTests
{
    public class GameControllerTests
    {
        private Mock<IGameService> _mockGameService;
        private IMapper _mapper;
        private GameController _controller;
        private Game _validGame01;
        private Game _validGame02;

        public GameControllerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
               cfg => cfg.AddProfile<GameMappingProfile>());
            _mapper = new Mapper(mapperConfiguration);
            _mockGameService = new Mock<IGameService>();
            _controller = new GameController(_mockGameService.Object, _mapper);
            _validGame01 = new Game()
            {
                Id = 10,
                Sport = SportType.Football,
                CreationDate = DateTime.Now,
                GameTime = DateTime.Now,
                Players = new List<Player>()
            };
            _validGame02 = new Game()
            {
                Id = 20,
                Sport = SportType.HandBall,
                CreationDate = DateTime.Now.AddYears(-1),
                GameTime = DateTime.Now.AddYears(1),
                Players = new List<Player>()
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOkObjectResultSuccessfulWithData()
        {
            //Arrange
            _mockGameService.Setup(service => service.GetAllGames())
                 .ReturnsAsync(new ServiceResult<IEnumerable<Game>>()
                 {
                     IsSuccess = true,
                     Data = new List<Game>() { _validGame01, _validGame02 },
                     Message = Common.Success
                 });
            // Act
            var result = await _controller.GetAllGames() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.True(response.IsSuccess == true);
            Assert.Equal(Common.Success, response.Status);
            Assert.NotNull(response.Content);
        }


        [Fact]
        public async Task GetAll_ReturnsOkObjectResultSuccessfulWithNoData()
        {
            //Arrange
            _mockGameService.Setup(service => service.GetAllGames())
                 .ReturnsAsync(new ServiceResult<IEnumerable<Game>>()
                 {
                     IsSuccess = true,
                     Data = new List<Game>(),
                     Message = Common.Success
                 });
            // Act
            var result = await _controller.GetAllGames() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.True(response.IsSuccess == true);
            Assert.Equal(Common.Success, response.Status);
            Assert.Empty(response.Content as IEnumerable<GameDto>);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequestResultOnError()
        {
            // Arrange
            _mockGameService.Setup(service => service.GetAllGames())
                 .ReturnsAsync(new ServiceResult<IEnumerable<Game>>()
                 {
                     IsSuccess = false,
                     Data = null,
                     Message = Common.SomethingWentWrong
                 });

            // Act
            var result = await _controller.GetAllGames() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.Error, response.Status);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.True(response.IsSuccess == false);
            Assert.Null(response.Content);
        }

        [Fact]
        public async Task GetById_ValidId_ReturnsOkResultWithData()
        {
            //Arrange
            _mockGameService.Setup(service => service.GetGameById(_validGame01.Id))
                 .ReturnsAsync(new ServiceResult<Game>()
                 {
                     IsSuccess = true,
                     Data = _validGame01,
                     Message = Common.Success
                 });
            // Act
            var result = await _controller.GetGameById(_validGame01.Id) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.True(response.IsSuccess == true);
            Assert.Equal(Common.Success, response.Status);
            Assert.NotNull(response.Content);
            Assert.True(((GameDto)response.Content).CreationDate == _validGame01.CreationDate);
        }

        [Fact]
        public async Task GetById_InvalidId_ShouldReturnNotFoundWithNoData()
        {
            // Arrange
            var notExistingGameId = 25;
            _mockGameService.Setup(service => service.GetGameById(notExistingGameId))
                 .ReturnsAsync(new ServiceResult<Game>()
                 {
                     IsSuccess = false,
                     Data = null,
                     Message = Common.GameNotExists
                 });

            // Act
            var result = await _controller.GetGameById(notExistingGameId) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.Error, response.Status);
            Assert.Equal(Common.GameNotExists, response.Message);
            Assert.True(response.IsSuccess == false);
            Assert.Null(response.Content);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequestResultOnErrorWithNoData()
        {
            // Arrange
            _mockGameService.Setup(service => service.GetGameById(_validGame01.Id))
                 .ReturnsAsync(new ServiceResult<Game>()
                 {
                     IsSuccess = false,
                     Data = null,
                     Message = Common.Error
                 });

            // Act
            var result = await _controller.GetGameById(_validGame01.Id) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.Error, response.Status);
            Assert.Equal(Common.Error, response.Message);
            Assert.True(response.IsSuccess == false);
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
                _mockGameService.Setup(service => service.DeleteGame(id))
                    .ReturnsAsync(new ServiceResult<Game>()
                    {
                        IsSuccess = true,
                        Data = null,
                        Message = Common.SuccessfullyDeleted
                    });
            }
            else
            {
                if (id == 69)
                {
                    _mockGameService.Setup(service => service.DeleteGame(id))
                        .ReturnsAsync(new ServiceResult<Game>()
                        {
                            IsSuccess = false,
                            Data = null,
                            Message = Common.GameNotExists
                        });
                }
                else
                {
                    _mockGameService.Setup(service => service.DeleteGame(id)).ReturnsAsync(new ServiceResult<Game>()
                    {
                        IsSuccess = false,
                        Data = null,
                        Message = Common.SomethingWentWrong
                    });
                }
            }

            // Act
            var result = await _controller.DeleteGame(id) as ObjectResult;

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
                _mockGameService.Verify(service => service.DeleteGame(id), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.GameNotExists, response.Message);
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
        public async Task Create_WithValidData_ReturnsObjectResultWithSuccessWithData()
        {
            // Arrange
            var newGameDto = new CreateGameInfoDto { Sport = SportType.BasketBall, GameTime = DateTime.Today };
            var game = _mapper.Map<CreateGameInfoDto, Game>(newGameDto);

            var expectedServiceResult = new ServiceResult<Game>
            {
                IsSuccess = true,
                Data = null,
                Message = Common.SuccessfullyCreated
            };

            _mockGameService.Setup(service => service.CreateGame(It.IsAny<Game>())).ReturnsAsync(expectedServiceResult);

            // Act
            var result = await _controller.CreateGame(newGameDto) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);

            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.Equal(Common.SuccessfullyCreated, response.Message);
            Assert.Equal(Common.Success, response.Status);
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Content);

            _mockGameService.Verify(service => service.CreateGame(It.IsAny<Game>()), Times.Once);
        }

        [Fact]
        public async Task Create_ValidData_ReturnsBadRequestResultOnErrorWithNoData()
        {
            // Arrange
            var newGame = new CreateGameInfoDto { Sport = SportType.BasketBall, GameTime = DateTime.Today };
            var expectedServiceResult = new ServiceResult<Game>
            {
                IsSuccess = false,
                Data = null,
                Message = Common.SomethingWentWrong
            };

            _mockGameService.Setup(service => service.CreateGame(It.IsAny<Game>())).ReturnsAsync(expectedServiceResult);

            // Act
            var result = await _controller.CreateGame(newGame) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);

            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.True(!response.IsSuccess);
            Assert.Null(response.Content);

            _mockGameService.Verify(service => service.CreateGame(It.IsAny<Game>()), Times.Once);
        }

        [Fact]
        public async Task UpdateGame_ValidId_Returns200Ok()
        {
            // Arrange
            var updateGameInfo = new UpdateGameInfoDto();
            var id = 10;

            var serviceResult = new ServiceResult<Game>
            {
                IsSuccess = true,
                Message = Common.Success
            };

            _mockGameService.Setup(service => service.UpdateGame(It.IsAny<Game>(), id)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateGame(updateGameInfo, id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            var response = Assert.IsType<Response>(objectResult.Value);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.Equal(Common.Success, response.Status);
            Assert.Equal(updateGameInfo, response.Content);
            Assert.Equal(serviceResult.Message, response.Message);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task UpdateGame_GameNotExists_Returns404NotFound()
        {
            // Arrange
            var updateGameInfo = new UpdateGameInfoDto();
            var id = 10;
            var serviceResult = new ServiceResult<Game>
            {
                IsSuccess = false,
                Message = Common.GameNotExists
            };

            _mockGameService.Setup(service => service.UpdateGame(It.IsAny<Game>(), id)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateGame(updateGameInfo, id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, objectResult.StatusCode);

            var response = Assert.IsType<Response>(objectResult.Value);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.Content);
            Assert.Equal(serviceResult.Message, response.Message);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task UpdateGame_BadRequest_Returns400BadRequest()
        {
            // Arrange
            var updateGameInfo = new UpdateGameInfoDto();
            var id = 10;
            var serviceResult = new ServiceResult<Game>
            {
                IsSuccess = false,
                Message = "Some other error"
            };

            _mockGameService.Setup(service => service.UpdateGame(It.IsAny<Game>(), id)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateGame(updateGameInfo, id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status400BadRequest, objectResult.StatusCode);

            var response = Assert.IsType<Response>(objectResult.Value);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.Error, response.Status);
            Assert.Null(response.Content);
            Assert.Equal(serviceResult.Message, response.Message);
            Assert.False(response.IsSuccess);
        }
    }
}
