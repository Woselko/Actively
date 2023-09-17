using ActivelyApp.Models.AuthenticationDto.Email;
using ActivelyApp.Models.AuthenticationDto.Login;
using ActivelyApp.Models.AuthenticationDto.PasswordReset;
using ActivelyApp.Models.AuthenticationDto.Registration;
using ActivelyApp.Models.Common;
using ActivelyApp.Services.UserServices.EmailService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Resources;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ActivelyApp.Controllers.Authentication
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager,
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
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser, string role = "User")
        {
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { Status = Common.Error, Message = Common.UserExist, Type = ResponseType.Error });
            }

            var user = new IdentityUser()
            {
                Email = registerUser.Email,
                UserName = registerUser.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = Common.Error, Message = Common.SomethingWentWrong, Type = ResponseType.Error });
            }

            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(x => x.Code == "InvalidUserName"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { Status = Common.Error, Message = Common.InvalidUserName, Type = ResponseType.Error });
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = Common.Error, Message = Common.SomethingWentWrong, Type = ResponseType.Error });
            }

            await _userManager.AddToRoleAsync(user, role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
            var message = new EmailMessage(new string[] { user.Email! }, Common.ActivelyActivationLink, Common.ActivelyClickActivationLink + confirmationLink);
            await _emailService.SendEmail(message);

            return StatusCode(StatusCodes.Status201Created,
                    new Response { Status = Common.Success, Message = Common.AccountCreated + " " + Common.EmailVerificationSentSucces, Type = ResponseType.Success });
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { Status = Common.Success, Message = Common.AccountActivated, Type = ResponseType.Success });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response { Status = Common.Error, Message = Common.UserDoesNotExist, Type = ResponseType.Error });

        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _userManager.FindByNameAsync(loginModel.UserName);
            if (user != null && user.TwoFactorEnabled && await _userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                await _signInManager.SignOutAsync();
                await _signInManager.PasswordSignInAsync(user, loginModel.Password, false, true);
                var token = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");

                var message = new EmailMessage(new string[] { user.Email! }, "OTP Confrimation", token);
                await _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status200OK,
                 new Response { Status = "Success", Message = $"We have sent an OTP to your Email {user.Email}", Type = ResponseType.Success });
            }
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password) && await _userManager.IsEmailConfirmedAsync(user))
            {
                var jwtToken = await BuildJWTToken(user);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                    expiration = jwtToken.ValidTo
                });

            }
            return Unauthorized(new Response { Status = Common.Error, Message = Common.LoginFailed, Type = ResponseType.Error });
        }

        [HttpPost]
        [Route("login-2FA")]
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    var jwtToken = await BuildJWTToken(user);

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                        expiration = jwtToken.ValidTo
                    });
                }
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { Status = Common.Error, Message = Common.InvalidCode, Type = ResponseType.Error });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var forgotPasswordLink = Url.Action(nameof(ResetPassword), "Authentication", new { token, email = user.Email }, Request.Scheme);

                var message = new EmailMessage(new string[] { user.Email! }, Common.ActivelyPasswordRecovery, Common.ActivelyPasswordRecoveryLink + forgotPasswordLink);
                await _emailService.SendEmail(message);

                return StatusCode(StatusCodes.Status201Created,
                        new Response { Status = Common.Success, Message = Common.PasswordResetRequest + user.Email, Type = ResponseType.Success });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = Common.Error, Message = Common.UserDoesNotExist, Type = ResponseType.Error });
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string token, string email)
        {
            var passwordResetModel = new PasswordReset { Token = token, Email = email };
            return Ok(new
            {
                passwordResetModel
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(PasswordReset passwordReset)
        {
            var user = await _userManager.FindByEmailAsync(passwordReset.Email);
            if (user != null)
            {
                var resetPasswordResult = await _userManager.ResetPasswordAsync(user, passwordReset.Token, passwordReset.Password);
                if (!resetPasswordResult.Succeeded)
                {
                    foreach (var error in resetPasswordResult.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return BadRequest(ModelState);
                }
                return StatusCode(StatusCodes.Status200OK,
                        new Response { Status = Common.Success, Message = Common.PasswordChangedSuccessfully, Type = ResponseType.Success });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { Status = Common.Error, Message = Common.UserDoesNotExist, Type = ResponseType.Error });
        }

        private async Task<JwtSecurityToken> BuildJWTToken(IdentityUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = GetToken(authClaims);
            return jwtToken;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(2),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
        }
    }
}
