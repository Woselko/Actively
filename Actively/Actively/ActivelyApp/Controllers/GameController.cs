using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<IEnumerable<Game>>> GetAll()
        {
            IEnumerable<Game> games = new List<Game>();

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
            Game game;
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
                return BadRequest("Something went wrong");
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
            return Ok("Successfully Updated");
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

            return Ok("Successfully Deleted");
        }

    }
}
