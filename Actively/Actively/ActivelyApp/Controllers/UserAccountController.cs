using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ActivelyApp.Services.UserServices.EmailService;
using Resources;
using ActivelyApp.Models.Common;

namespace ActivelyApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UserAccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public UserAccountController(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager, IEmailService emailService,
            SignInManager<IdentityUser> signInManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("set-2FA")]
        public async Task<IActionResult> SetEmailTwoFactorAuthentication(bool enableTwoFactorAuth)
        {
            var id = User.Claims.First().Value;
            var userExist = await _userManager.FindByIdAsync(id);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { IsSuccess = false, Message = Common.UserExist,  });
            }
            await _userManager.SetTwoFactorEnabledAsync(userExist, enableTwoFactorAuth);

            return Ok();
        }
    }
}
