using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;

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
        public async Task<ActionResult<IEnumerable<PlayerDto>>> GetAll()
        {
            IEnumerable<PlayerDto> players = null;

            try
            {
                players = await _playerService.GetAll();
                if (players == null)
                {
                    return BadRequest(Common.SomethingWentWrong);
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(players);
        }

        [HttpGet]
        public async Task<ActionResult<PlayerDto>> GetById(int id)
        {
            PlayerDto player = null;
            try
            {
                player = await _playerService.GetById(id);
                if (player == null)
                {
                    return NotFound(Common.GameNotExistsError);
                }
            }
            catch (NotFoundEntityException)
            {
                //log
            }
            catch (Exception e)
            {
                //log
                return BadRequest(e.Message);
            }

            return Ok(player);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreatePlayerInfoDto newPlayer)
        {          
            if (newPlayer == null)
            {
                return BadRequest(Common.SomethingWentWrong);
            }
            try
            {
                await _playerService.Create(newPlayer);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return StatusCode(201);
        }

        [HttpPatch]
        public async Task<ActionResult> Update([FromBody] UpdatePlayerInfoDto updatePlayerInfo, int id)
        {        
            if (updatePlayerInfo == null)
                return BadRequest(Common.SomethingWentWrong);
            try
            {
                await _playerService.Update(updatePlayerInfo, id);
            }
            catch (NotFoundEntityException)
            {
                return NotFound(Common.PlayerNotExistsError);
            }
            catch (Exception)
            {
                return BadRequest(Common.SomethingWentWrong);
            }
            return Ok(Common.SuccessfullyUpdated);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {            
            try
            {
                await _playerService.Delete(id);
            }
            catch (NotFoundEntityException)
            {
                return NotFound(Common.PlayerNotExistsError);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok(Common.SuccessfullyDeleted);
        }
    }
}
