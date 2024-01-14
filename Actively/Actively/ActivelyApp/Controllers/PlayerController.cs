using ActivelyApp.CustomExceptions;
using ActivelyApp.Models.Common;
using ActivelyApp.Models.EntityDto;
using ActivelyApp.Services.EntityService;
using ActivelyDomain.Entities;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public PlayerController(IPlayerService playerService, IMapper mapper)
        {
            _playerService = playerService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var serviceResult = await _playerService.GetAllPlayers();
            if (serviceResult.IsSuccess == true)
            {
                var players = _mapper.Map<IEnumerable<PlayerDto>>(serviceResult.Data);
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = players, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayerById(int id)
        {
            var serviceResult = await _playerService.GetPlayerById(id);

            if (serviceResult.IsSuccess == true)
            {
                var player = _mapper.Map<PlayerDto>(serviceResult.Data);
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = player, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                if (serviceResult.Message == Common.PlayerNotExists)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
                }
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerInfoDto newPlayer)
        {
            var entityNewPlayer = _mapper.Map<Player>(newPlayer);
            var serviceResult = await _playerService.CreatePlayer(entityNewPlayer);
            if (serviceResult.IsSuccess == true)
            {
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = newPlayer, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }
        }

        [HttpPatch]
        public async Task<IActionResult> UpdatePlayer([FromBody] UpdatePlayerInfoDto updatePlayerInfo, int id)
        {
            var entityUpdatePlayer = _mapper.Map<Player>(updatePlayerInfo);
            var serviceResult = await _playerService.UpdatePlayer(entityUpdatePlayer, id);
            if (serviceResult.IsSuccess == true)
            {
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Content = updatePlayerInfo, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                if (serviceResult.Message == Common.PlayerNotExists)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new Response
                    { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
                }

                return StatusCode(StatusCodes.Status400BadRequest, new Response
                { Type = ResponseType.Error, Status = Common.Error, Message = serviceResult.Message, IsSuccess = false });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            var serviceResult = await _playerService.DeletePlayer(id);
            if (serviceResult.IsSuccess == true)
            {
                return StatusCode(StatusCodes.Status200OK, new Response
                { Type = ResponseType.Success, Status = Common.Success, Message = serviceResult.Message, IsSuccess = true });
            }
            else
            {
                if (serviceResult.Message == Common.PlayerNotExists)
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
