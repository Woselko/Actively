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
    public class PlayerControllerTests
    {
        private Mock<IPlayerService> _mockPlayerService;
        private IMapper _mapper;
        private PlayerController _controller;
        private Player _validPlayer01;
        private Player _validPlayer02;
        public PlayerControllerTests()
        {
            var mapperConfiguration = new MapperConfiguration(
               cfg => cfg.AddProfile<PlayerMappingProfile>());
            _mapper = new Mapper(mapperConfiguration);
            _mockPlayerService = new Mock<IPlayerService>();
            _controller = new PlayerController(_mockPlayerService.Object, _mapper);
            _validPlayer01 = new Player()
            {
                Id = 10,
                FirstName = "FTest01",
                LastName = "LTest01",
                Games = new List<Game>(),
                NickName = "Test01"

            };
            _validPlayer02 = new Player()
            {
                Id = 20,
                FirstName = "FTest02",
                LastName = "LTest02",
                Games = new List<Game>(),
                NickName = "Test01"
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOkObjectResultSuccessfulWithData()
        {
            //Arrange
            _mockPlayerService.Setup(service => service.GetAllPlayers())
                 .ReturnsAsync(new ServiceResult<IEnumerable<Player>>()
                 {
                     IsSuccess = true,
                     Data = new List<Player>() { _validPlayer01, _validPlayer02 },
                     Message = Common.Success
                 });
            // Act
            var result = await _controller.GetAllPlayers() as ObjectResult;

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
            _mockPlayerService.Setup(service => service.GetAllPlayers())
                 .ReturnsAsync(new ServiceResult<IEnumerable<Player>>()
                 {
                     IsSuccess = true,
                     Data = new List<Player>(),
                     Message = Common.Success
                 });
            // Act
            var result = await _controller.GetAllPlayers() as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.True(response.IsSuccess == true);
            Assert.Equal(Common.Success, response.Status);
            Assert.Empty(response.Content as IEnumerable<PlayerDto>);
        }

        [Fact]
        public async Task GetAll_ReturnsBadRequestResultOnError()
        {
            // Arrange
            _mockPlayerService.Setup(service => service.GetAllPlayers())
                 .ReturnsAsync(new ServiceResult<IEnumerable<Player>>()
                 {
                     IsSuccess = false,
                     Data = null,
                     Message = Common.SomethingWentWrong
                 });

            // Act
            var result = await _controller.GetAllPlayers() as ObjectResult;

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
            _mockPlayerService.Setup(service => service.GetPlayerById(_validPlayer01.Id))
                 .ReturnsAsync(new ServiceResult<Player>()
                 {
                     IsSuccess = true,
                     Data = _validPlayer01,
                     Message = Common.Success
                 });
            // Act
            var result = await _controller.GetPlayerById(_validPlayer01.Id) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.True(response.IsSuccess == true);
            Assert.Equal(Common.Success, response.Status);
            Assert.NotNull(response.Content);
            Assert.True(((PlayerDto)response.Content).NickName == _validPlayer01.NickName);
        }

        [Fact]
        public async Task GetById_InvalidId_ShouldReturnNotFoundWithNoData()
        {
            // Arrange
            var notExistingPlayerId = 25;
            _mockPlayerService.Setup(service => service.GetPlayerById(notExistingPlayerId))
                 .ReturnsAsync(new ServiceResult<Player>()
                 {
                     IsSuccess = false,
                     Data = null,
                     Message = Common.PlayerNotExists
                 });

            // Act
            var result = await _controller.GetPlayerById(notExistingPlayerId) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.Error, response.Status);
            Assert.Equal(Common.PlayerNotExists, response.Message);
            Assert.True(response.IsSuccess == false);
            Assert.Null(response.Content);
        }

        [Fact]
        public async Task GetById_ReturnsBadRequestResultOnErrorWithNoData()
        {
            // Arrange
            _mockPlayerService.Setup(service => service.GetPlayerById(_validPlayer01.Id))
                 .ReturnsAsync(new ServiceResult<Player>()
                 {
                     IsSuccess = false,
                     Data = null,
                     Message = Common.Error
                 });

            // Act
            var result = await _controller.GetPlayerById(_validPlayer01.Id) as ObjectResult;

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
                _mockPlayerService.Setup(service => service.DeletePlayer(id))
                    .ReturnsAsync(new ServiceResult<Player>()
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
                    _mockPlayerService.Setup(service => service.DeletePlayer(id))
                        .ReturnsAsync(new ServiceResult<Player>()
                        {
                            IsSuccess = false,
                            Data = null,
                            Message = Common.PlayerNotExists
                        });
                }
                else
                {
                    _mockPlayerService.Setup(service => service.DeletePlayer(id)).ReturnsAsync(new ServiceResult<Player>()
                    {
                        IsSuccess = false,
                        Data = null,
                        Message = Common.SomethingWentWrong
                    });
                }
            }

            // Act
            var result = await _controller.DeletePlayer(id) as ObjectResult;

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
                _mockPlayerService.Verify(service => service.DeletePlayer(id), Times.Once);
            }
            else
            {
                if (id == 69)
                {
                    var response = Assert.IsType<Response>(result.Value);
                    Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
                    Assert.Equal(ResponseType.Error, response.Type);
                    Assert.Equal(Common.PlayerNotExists, response.Message);
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
            var newPlayerDto = new CreatePlayerInfoDto { FirstName = "Test01", LastName = "Test01", NickName = "Test01" };
            var player = _mapper.Map<CreatePlayerInfoDto, Player>(newPlayerDto);

            var expectedServiceResult = new ServiceResult<Player>
            {
                IsSuccess = true,
                Data = null,
                Message = Common.SuccessfullyCreated
            };

            _mockPlayerService.Setup(service => service.CreatePlayer(It.IsAny<Player>())).ReturnsAsync(expectedServiceResult);

            // Act
            var result = await _controller.CreatePlayer(newPlayerDto) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.Equal(Common.SuccessfullyCreated, response.Message);
            Assert.Equal(Common.Success, response.Status);
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Content);
            _mockPlayerService.Verify(service => service.CreatePlayer(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task Create_ValidData_ReturnsBadRequestResultOnErrorWithNoData()
        {
            // Arrange
            var newPlayer = new CreatePlayerInfoDto { FirstName = "Test01", LastName = "Test01", NickName = "Test01" };
            var expectedServiceResult = new ServiceResult<Player>
            {
                IsSuccess = false,
                Data = null,
                Message = Common.SomethingWentWrong
            };

            _mockPlayerService.Setup(service => service.CreatePlayer(It.IsAny<Player>())).ReturnsAsync(expectedServiceResult);

            // Act
            var result = await _controller.CreatePlayer(newPlayer) as ObjectResult;

            // Assert
            var response = Assert.IsType<Response>(result.Value);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(ResponseType.Error, response.Type);
            Assert.Equal(Common.SomethingWentWrong, response.Message);
            Assert.Equal(Common.Error, response.Status);
            Assert.True(!response.IsSuccess);
            Assert.Null(response.Content);
            _mockPlayerService.Verify(service => service.CreatePlayer(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task UpdatePlayer_ValidId_Returns200Ok()
        {
            // Arrange
            var updatePlayerInfo = new UpdatePlayerInfoDto();
            var id = 10;
            var serviceResult = new ServiceResult<Player>
            {
                IsSuccess = true,
                Message = Common.Success
            };

            _mockPlayerService.Setup(service => service.UpdatePlayer(It.IsAny<Player>(), id)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdatePlayer(updatePlayerInfo, id);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, objectResult.StatusCode);

            var response = Assert.IsType<Response>(objectResult.Value);
            Assert.Equal(ResponseType.Success, response.Type);
            Assert.Equal(Common.Success, response.Status);
            Assert.Equal(updatePlayerInfo, response.Content);
            Assert.Equal(serviceResult.Message, response.Message);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task UpdatePlayer_PlayerNotExists_Returns404NotFound()
        {
            // Arrange
            var updatePlayerInfo = new UpdatePlayerInfoDto();
            var id = 10;
            var serviceResult = new ServiceResult<Player>
            {
                IsSuccess = false,
                Message = Common.PlayerNotExists
            };

            _mockPlayerService.Setup(service => service.UpdatePlayer(It.IsAny<Player>(), id)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdatePlayer(updatePlayerInfo, id);

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
        public async Task UpdatePlayer_BadRequest_Returns400BadRequest()
        {
            // Arrange
            var updatePlayerInfo = new UpdatePlayerInfoDto();
            var id = 10;
            var serviceResult = new ServiceResult<Player>
            {
                IsSuccess = false,
                Message = "Some other error"
            };

            _mockPlayerService.Setup(service => service.UpdatePlayer(It.IsAny<Player>(), id)).ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdatePlayer(updatePlayerInfo, id);

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
