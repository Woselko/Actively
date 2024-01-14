using ActivelyApp.Models.Common;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;

namespace ActivelyApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class GameController : Controller
    {
        private readonly IGameService _gameService;
        private readonly IMapper _mapper;

        public GameController(IGameService gameService, IMapper mapper)
        {
            _gameService = gameService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var serviceResult = await _gameService.GetAllGames();
            if (serviceResult.IsSuccess == true)
            {
                var games = _mapper.Map<IEnumerable<GameDto>>(serviceResult.Data);
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = games, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }
        }
         
        [HttpGet]
        public async Task<IActionResult> GetGameById(int id)
        {
            var serviceResult = await _gameService.GetGameById(id);

            if (serviceResult.IsSuccess == true)
            {
                var game = _mapper.Map<GameDto>(serviceResult.Data);
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = game, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                if (serviceResult.Message == Common.GameNotExists)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
                }
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] CreateGameInfoDto newGame)
        {
            var entityNewGame = _mapper.Map<Game>(newGame);
            var serviceResult = await _gameService.CreateGame(entityNewGame);
            if (serviceResult.IsSuccess == true)
            {
                return StatusCode(StatusCodes.Status201Created, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = newGame, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }  
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateGame([FromBody] UpdateGameInfoDto updateGameInfo, int id)
        {
            var entityUpdateGame = _mapper.Map<Game>(updateGameInfo);
            var serviceResult = await _gameService.UpdateGame(entityUpdateGame, id);
            if (serviceResult.IsSuccess == true)
            {
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = updateGameInfo, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                if (serviceResult.Message == Common.GameNotExists)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
                }

                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var serviceResult = await _gameService.DeleteGame(id);
            if (serviceResult.IsSuccess == true)
            {
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                if (serviceResult.Message == Common.GameNotExists)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
                }

                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }
        }
    }
}
