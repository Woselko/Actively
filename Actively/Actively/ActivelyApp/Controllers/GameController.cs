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
        public async Task<ActionResult<IEnumerable<GameDto>>> GetAll()
        {
            IEnumerable<GameDto> games = new List<GameDto>();

            try
            {
                games = await _gameService.GetAll();
                if (games == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = Common.GameNotExistsError });
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message });
            }

            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Succes, Status = Common.Success, ReturnObject = games });
        }
         
        [HttpGet]
        public async Task<ActionResult> GetById(int id)
        {
            GameDto game;
            try
            {
                game = await _gameService.GetById(id);
                if (game == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = Common.GameNotExistsError });
                }

            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message });
            }


            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Succes, Status = Common.Success, ReturnObject = game });
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateGameInfoDto newGame)
        {
            if (newGame == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.SomethingWentWrong });
            }
            try
            {
                await _gameService.Create(newGame);
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.SomethingWentWrong, Message = e.Message });
            }

            return StatusCode(StatusCodes.Status201Created, new Response
            { Type = ResponseType.Succes, Status = Common.Success, ReturnObject = newGame });
        }

        [HttpPatch]
        public async Task<ActionResult> Update([FromBody] UpdateGameInfoDto updateGameInfo, int id)
        {
            if (updateGameInfo == null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.SomethingWentWrong });
            try
            {
                await _gameService.Update(updateGameInfo, id);               
            }
            catch (NotFoundEntityException)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.GameNotExistsError });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.SomethingWentWrong, Message = e.Message });
            }
            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Succes, Status = Common.Success, Message = Common.SuccessfullyUpdated });
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _gameService.Delete(id);
            }
            catch(NotFoundEntityException)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.PlayerNotExistsError });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Message = e.Message, Status = Common.SomethingWentWrong });
            }

            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Succes, Status = Common.Success, Message = Common.SuccessfullyDeleted });
        }

    }
}
