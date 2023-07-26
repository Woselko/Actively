using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
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
                    return NotFound();
                }

            }
            catch (NotFoundEntityException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(games);
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
                    return NotFound();
                }

            }
            catch (NotFoundEntityException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(game);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateGameInfo newGame)
        {
            if (newGame == null)
            {
                return BadRequest(Common.SomethingWentWrong);
            }
            try
            {
                await _gameService.Create(newGame);
            }
            catch (NotFoundEntityException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return StatusCode(201);
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateGameInfo updateGameInfo, int id)
        {
            if (updateGameInfo == null)
                return BadRequest(Common.SomethingWentWrong);
            try
            {
                await _gameService.Update(updateGameInfo, id);               
            }
            catch (NotFoundEntityException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok(Common.SuccessfullyUpdated);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _gameService.Delete(id);
            }
            catch(NotFoundEntityException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(Common.SuccessfullyDeleted);
        }

    }
}
