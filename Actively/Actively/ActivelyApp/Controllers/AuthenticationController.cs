using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ActivelyApp.Models.Authentication.Email;
using ActivelyApp.Models.Authentication.Login;
using ActivelyApp.Models.Authentication.Password;
using ActivelyApp.Models.Authentication.Registration;
using ActivelyApp.Services.UserServices.EmailService;
using Resources;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ActivelyApp.Models.Common;
using ActivelyDomain.Entities;
using System.Security.Cryptography;
using ActivelyApp.Models.Authentication.Authentication;

namespace ActivelyApp.Controllers.Authentication
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<User> userManager,
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
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new Response { IsSuccess = false, Message = Common.UserExist,  });
            }
            var role = "User";

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { IsSuccess = false, Message = Common.SomethingWentWrong,  });
            }
            var user = await CreateUser(registerUser);
            var result = await _userManager.CreateAsync(user, registerUser.Password);

            if (!result.Succeeded)
            {
                if (result.Errors.Any(x => x.Code == "InvalidUserName"))
                {
                    return StatusCode(StatusCodes.Status400BadRequest,
                        new Response { IsSuccess = false, Message = Common.InvalidUserName,  });
                }
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { IsSuccess = false, Message = Common.SomethingWentWrong,  });
            }

            await _userManager.AddToRoleAsync(user, role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Authentication", new { token, email = user.Email }, Request.Scheme);
            var message = new EmailMessage(new string[] { user.Email! }, Common.ActivelyActivationLink, Common.ActivelyClickActivationLink + confirmationLink);
            await _emailService.SendEmail(message);

            return StatusCode(StatusCodes.Status201Created,
                    new Response { IsSuccess = true, Message = Common.AccountCreated + " " + Common.EmailVerificationSentSucces,  });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return StatusCode(StatusCodes.Status200OK, new Response { IsSuccess = true, Message = Common.AccountActivated,  });
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError,
                   new Response { IsSuccess = false, Message = Common.UserDoesNotExist,  });

        }

        [HttpPost]
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
                 new Response { IsSuccess = true, Message = $"We have sent an OTP to your Email {user.Email}"});
            }
            if (user != null && await _userManager.CheckPasswordAsync(user, loginModel.Password) && await _userManager.IsEmailConfirmedAsync(user))
            {
                string accessToken = await GenerateAccessToken(user);
                var refreshToken = GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);

                var response = new Response
                {
                    Content = new AuthenticationResponse
                    {
                        RefreshToken = refreshToken,
                        AccessToken = accessToken
                    },
                    IsSuccess = true,
                    Message = Common.Success
                };
                return Ok(response);

            }
            return Unauthorized(new Response { IsSuccess = false, Message = Common.LoginFailed,  });
        }

        [HttpPost]
        public async Task<IActionResult> LoginWithOTP(string code, string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            var signIn = await _signInManager.TwoFactorSignInAsync("Email", code, false, false);
            if (signIn.Succeeded)
            {
                if (user != null)
                {
                    string accessToken = await GenerateAccessToken(user);
                    var refreshToken = GenerateRefreshToken();
                    user.RefreshToken = refreshToken;
                    await _userManager.UpdateAsync(user);

                    var response = new Response
                    {
                        Content = new AuthenticationResponse
                        {
                            RefreshToken = refreshToken,
                            AccessToken = accessToken
                        },
                        IsSuccess = true,
                        Message = Common.Success
                    };
                    return Ok(response);
                }
            }
            return StatusCode(StatusCodes.Status404NotFound,
                new Response { IsSuccess = false, Message = Common.InvalidCode + " " + Common.LoginFailed});
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
                        new Response { IsSuccess = true, Message = Common.PasswordResetRequest + user.Email,  });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { IsSuccess = false, Message = Common.UserDoesNotExist,  });
        }

        [HttpGet]
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
                        new Response { IsSuccess = true, Message = Common.PasswordChangedSuccessfully,  });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { IsSuccess = false, Message = Common.UserDoesNotExist,  });
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

        private async Task<string> GenerateAccessToken(User user)
        {
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Surname, $"{user.FirstName} { user.LastName}"),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim("UserAvatar", $"{user.UserAvatar}"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtToken = GetToken(authClaims);
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        private string GenerateRefreshToken()
        {

            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task<User> CreateUser(RegisterUser registerUser)
        {
            var user = new User()
            {
                Email = registerUser.Email,
                UserName = registerUser.Username,
                FirstName = registerUser.FirstName,
                LastName = registerUser.LastName,
                Address = registerUser.Address,
                Gender = registerUser.Gender,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            return user;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var response = new Response();
            if (refreshTokenRequest is null)
            {
                response.IsSuccess = false;
                response.Message = "Invalid  request";
                return BadRequest(response);
            }

            var principal = GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);

            if (principal != null)
            {
                var email = principal.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Email);

                var user = await _userManager.FindByEmailAsync(email?.Value);

                if (user is null || user.RefreshToken != refreshTokenRequest.RefreshToken)
                {
                    response.Message = "Invalid Request";
                    return BadRequest(response);
                }

                string newAccessToken = await GenerateAccessToken(user);
                string refreshToken = GenerateRefreshToken();

                user.RefreshToken = refreshToken;
                await _userManager.UpdateAsync(user);

                response.IsSuccess = true;
                response.Content = new AuthenticationResponse
                {
                    RefreshToken = refreshToken,
                    AccessToken = newAccessToken
                };
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var keyDetail = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(keyDetail),
            };

            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameter, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}
