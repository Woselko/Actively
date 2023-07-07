using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayMakerApp.Models;
using PlayMakerApp.Models.Authentication.Email;
using PlayMakerApp.Models.Authentication.Registration;
using PlayMakerApp.Services.UserServices.EmailService;

namespace PlayMakerApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AuthenticationController(IEmailService emailService,UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManage)
        {
            _emailService = emailService;
            _userManager = userManager;
            _roleManager = roleManage;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody, FromForm]RegisterUser registerUser, string role)
        {
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if(userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response {Status = Resources.Common.Error , Message = Resources.Common.UserExist });
            }

            var user = new IdentityUser()
            {
                Email = registerUser.Email,
                UserName = registerUser.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            if(!await _roleManager.RoleExistsAsync(role))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = Resources.Common.Error, Message = Resources.Common.SomethingWentWrong });
            }

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = Resources.Common.Error, Message = Resources.Common.SomethingWentWrong });
            }

            await _userManager.AddToRoleAsync(user,role);
            return StatusCode(StatusCodes.Status201Created,
                    new Response { Status = Resources.Common.Success, Message = Resources.Common.AccountCreated });
        }

        [HttpGet]
        public async Task<IActionResult> TestEmail()
        {
            var message = new EmailMessage(new string[] { "wojciech.wlas@gmail.com" }, "Test", "<h1>This is PlayMaker</h1>");
            await _emailService.SendEmail(message);
            return StatusCode(StatusCodes.Status200OK,
                new Response { Status = "OK", Message = "Email sent" });
        }
    }
}
