using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.Common;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System.Numerics;
using System.Runtime.CompilerServices;

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
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<PlayerDto> players = null;

            try
            {
                players = await _playerService.GetAll();
                if (players == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = Common.PlayerNotExistsError });
                }
            }
            catch (Exception e)
            {           
                return StatusCode(StatusCodes.Status400BadRequest, new Response 
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message });
            }

            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Success, Status = Common.Success, ReturnObject = players });           
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            PlayerDto player = null;
            try
            {
                player = await _playerService.GetById(id);
                if (player == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = Common.PlayerNotExistsError });
                }
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message });
            }
             
            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Success, Status = Common.Success, ReturnObject = player });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePlayerInfoDto newPlayer)
        {          
            if (newPlayer == null)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.SomethingWentWrong });
            }
            try
            {
                await _playerService.Create(newPlayer);
            }            
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message });
            }
            

            return StatusCode(StatusCodes.Status201Created, new Response 
            { Type = ResponseType.Success, Status = Common.Success, Message = Common.Success });
        }

        [HttpPatch]
        public async Task<IActionResult> Update([FromBody] UpdatePlayerInfoDto updatePlayerInfo, int id)
        {        
            if (updatePlayerInfo == null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.SomethingWentWrong });
            try
            {
                await _playerService.Update(updatePlayerInfo, id);
            }
            catch (NotFoundEntityException)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.PlayerNotExistsError });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message });
            }
            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Success, Status = Common.Success, Message = Common.SuccessfullyUpdated });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {            
            try
            {
                await _playerService.Delete(id);
            }
            catch (NotFoundEntityException)
            {
                return StatusCode(StatusCodes.Status404NotFound, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = Common.PlayerNotExistsError });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response 
                { Type = ResponseType.Error, Status = Common.Error, Message = e.Message });
            }

            return StatusCode(StatusCodes.Status200OK, new Response
            { Type = ResponseType.Success, Status = Common.Success, Message = Common.SuccessfullyDeleted });
        }
    }
}
