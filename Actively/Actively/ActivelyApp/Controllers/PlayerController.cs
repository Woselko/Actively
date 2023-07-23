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
            IEnumerable<Player> players = new List<Player>();

            try
            {
                players = await _playerService.GetAll();
                if (players == null)
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

            return Ok(players);
        }

        [HttpGet]
        public async Task<ActionResult> GetById(int id)
        {
            Player player;
            try
            {
                player = await _playerService.GetById(id);
                if (player == null)
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

            return Ok(player);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreatePlayerInfo newPlayer)
        {          
            if (newPlayer == null)
            {
                return BadRequest("Something went wrong");
            }
            try
            {
                await _playerService.Create(newPlayer);
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
        public async Task<ActionResult> Update([FromBody] UpdatePlayerInfo updatePlayerInfo, int id)
        {        
            try
            {
                await _playerService.Update(updatePlayerInfo, id);
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
                await _playerService.Delete(id);
            }
            catch (NotFoundEntityException e)
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
