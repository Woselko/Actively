using ActivelyApp.Models.Entity;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActivelyApp.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]/[action]")]
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetAll()
        {
            var players = await _playerService.GetAll();
            return Ok(players);
        }
        [HttpGet]
        public async Task<ActionResult> GetById(int id)
        {
            var player = await _playerService.GetById(id);
            return Ok(player);
        }
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreatePlayerInfo newPlayer)
        {

            await _playerService.Create(newPlayer);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdatePlayerInfo updatePlayerInfo, int id)
        {
            await _playerService.Update(updatePlayerInfo, id);
            return Ok();
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            await _playerService.Delete(id);
            return Ok();
        }
    }
}
