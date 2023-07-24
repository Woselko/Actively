using ActivelyApp.Models.Admin;
using ActivelyApp.Models.Common;
using ActivelyApp.Services.UserServices.EmailService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System.Data;

namespace ActivelyApp.Controllers
{
    [Authorize(Roles = "Admin, Developer, Moderator")]
    [Route("[controller]/[action]")]
    public class AdminOperationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AdminOperationController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, IEmailService emailService,
            SignInManager<User> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(RoleToUserDto assignRoleToUserDTO)
        {
            if (!await _roleManager.RoleExistsAsync(assignRoleToUserDTO.RoleName))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { IsSuccess = false, Message = Common.SomethingWentWrong });
            }
            var user = await _userManager.FindByEmailAsync(assignRoleToUserDTO.Email);

            if (user != null)
            {
                var userRoleAssignResponse = await _userManager.AddToRoleAsync(user, assignRoleToUserDTO.RoleName);
                if (userRoleAssignResponse.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { IsSuccess = true, Message = Common.Success });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { IsSuccess = false, Message = Common.SomethingWentWrong });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { IsSuccess = false, Message = Common.UserDoesNotExist });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveRoleFromUser(RoleToUserDto removeRoleFromUser)
        {
            if (!await _roleManager.RoleExistsAsync(removeRoleFromUser.RoleName))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { IsSuccess = false, Message = Common.SomethingWentWrong });
            }
            var user = await _userManager.FindByEmailAsync(removeRoleFromUser.Email);

            if (user != null)
            {
                var userRoleAssignResponse = await _userManager.RemoveFromRoleAsync(user, removeRoleFromUser.RoleName);
                if (userRoleAssignResponse.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new Response { IsSuccess = true, Message = Common.Success });
                }
                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new Response { IsSuccess = false, Message = Common.SomethingWentWrong });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { IsSuccess = false, Message = Common.UserDoesNotExist });
            }
        }
    }
}
