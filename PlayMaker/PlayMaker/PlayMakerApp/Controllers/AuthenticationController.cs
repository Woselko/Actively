using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayMakerApp.Models;
using PlayMakerApp.Models.Authentication.Registration;

namespace PlayMakerApp.Controllers
{
    //[ApiController]
    //[Route("api/[Controller]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, 
            RoleManager<IdentityRole> roleManage, IConfiguration configuration)
        {
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManage;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody, FromForm]RegisterUser registerUser, string role)
        {
            //Check if user exist
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if(userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response {Status = Resources.Common.Error , Message = Resources.Common.UserExist });
            }

            //Adding user to DB
            var user = new IdentityUser()
            {
                Email = registerUser.Email,
                UserName = registerUser.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            return result.Succeeded
                ? StatusCode(StatusCodes.Status201Created, 
                    new Response { Status = Resources.Common.Success, Message = Resources.Common.AccountCreated })
                : StatusCode(StatusCodes.Status500InternalServerError, 
                    new Response { Status = Resources.Common.Error, Message = Resources.Common.SomethingWentWrong });
        }
    }
}
