using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ActivelyApp.Services.UserServices.EmailService;
using Resources;
using ActivelyApp.Models.Common;
using ActivelyDomain.Entities;
using ActivelyApp.Models.Admin;
using System.Data;
using ActivelyApp.Models.Authentication.Password;

namespace ActivelyApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserAccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserAccountController(UserManager<User> userManager,
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
        public async Task<IActionResult> SetEmailTwoFactorAuthentication(bool enableTwoFactorAuth)
        {
            var id = User.Claims.First().Value;
            var userExist = await _userManager.FindByIdAsync(id);
            if (userExist == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { IsSuccess = false, Message = Common.SomethingWentWrong});
            }
            await _userManager.SetTwoFactorEnabledAsync(userExist, enableTwoFactorAuth);
            return Ok(new Response { IsSuccess = false, Message = enableTwoFactorAuth? Common.TwoFactorAuthIsSet : Common.TwoFactorAuthIsOff });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePassword changePasswordDto)
        {
            var id = User.Claims.First().Value;
            var userExist = await _userManager.FindByIdAsync(id);
            if (userExist == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { IsSuccess = false, Message = Common.SomethingWentWrong});
            }
            var result = await _userManager.ChangePasswordAsync(userExist,changePasswordDto.OldPassword,changePasswordDto.NewPassword);
            if (result.Succeeded)
            {
                return Ok(new Response { IsSuccess = false, Message = Common.PasswordChangedSuccessfully});
            }
            return StatusCode(StatusCodes.Status500InternalServerError, new Response { IsSuccess = false, Message = Common.SomethingWentWrong, });
        }
    }
}
