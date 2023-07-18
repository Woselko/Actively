using ActivelyApp.Models.Entity;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            var games = await _gameService.GetAll();
            return Ok(games);
        }
        [HttpGet]
        public async Task<ActionResult> GetById(int id)
        {
            var game = await _gameService.GetById(id);
            return Ok(game);
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateGameInfo newGame)
        {

            await _gameService.Create(newGame);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateGameInfo updateGameInfo, int id)
        {
            await _gameService.Update(updateGameInfo, id);
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            await _gameService.Delete(id);
            return Ok();
        }

    }
}
