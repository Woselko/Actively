using ActivelyApp.CustomExceptions;
using ActivelyApp.Mappings;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using ActivelyInfrastructure.Repositories.EntityRepositories.GameRepository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            _mapper = new Mapper(mapperConfiguration);
            _gameRepository = new Mock<IGameRepository>();
            _gameService = new GameService(_gameRepository.Object, _mapper);
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
        public async Task GetAll_ReturnsNotNullCollecitonAndCorrectGamesCount()
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
        public async Task GetById_ReturnsExpectedResponse(int id, bool isValid)
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
    }
}
