using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.Common;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System.Numerics;

namespace ActivelyApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class GameController : Controller
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<GameDto> games = new List<GameDto>();

            try
            {
                games = await _gameService.GetAll();
                if (games == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = Common.GameNotExistsError, IsSuccess = false });
                }

                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = games, IsSuccess = true});
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message, IsSuccess = false });
            }

            
        }
         
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            GameDto game;
            try
            {
                game = await _gameService.GetById(id);
                if (game == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = Common.GameNotExistsError, IsSuccess = false });
                }

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message, IsSuccess = false });
            }


            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Success, Status = Common.Success, Content = game, IsSuccess = true});
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameInfoDto newGame)
        {
            if (newGame == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.SomethingWentWrong, IsSuccess = false });
            }
            try
            {
                await _gameService.Create(newGame);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message, IsSuccess = false });
            }

            return StatusCode(StatusCodes.Status201Created, new Response
            { Type = ResponseType.Success, Status = Common.Success, Message = Common.Success, IsSuccess = true});
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdateGameInfoDto updateGameInfo, int id)
        {
            if (updateGameInfo == null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.SomethingWentWrong, IsSuccess = false });
            try
            {
                await _gameService.Update(updateGameInfo, id);               
            }
            catch (NotFoundEntityException)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.GameNotExistsError, IsSuccess = false });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message });
            }
            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Success, Status = Common.Success, Message = Common.SuccessfullyUpdated, IsSuccess = true });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _gameService.Delete(id);
            }
            catch(NotFoundEntityException)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.GameNotExistsError, IsSuccess = false });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message, IsSuccess = false });
            }

            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Success, Status = Common.Success, Message = Common.SuccessfullyDeleted, IsSuccess = true });
        }

    }
}
