using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ActivelyApp.Models.Authentication.Email;
using ActivelyApp.Models.Authentication.Login;
using ActivelyApp.Models.Authentication.PasswordReset;
using ActivelyApp.Models.Authentication.Registration;
using ActivelyApp.Services.UserServices.EmailService;
using Resources;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ActivelyApp.Models.Common;
using ActivelyDomain.Entities;

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
            var user = await CreateUser(registerUser);

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { IsSuccess = false, Message = Common.SomethingWentWrong,  });
            }

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

        [HttpGet("ConfirmEmail")]
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
                 new Response { IsSuccess = true, Message = $"We have sent an OTP to your Email {user.Email}"});
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
            return Unauthorized(new Response { IsSuccess = false, Message = Common.LoginFailed,  });
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
                new Response { IsSuccess = false, Message = Common.InvalidCode,  });
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
                        new Response { IsSuccess = true, Message = Common.PasswordChangedSuccessfully,  });
            }
            else
                return StatusCode(StatusCodes.Status400BadRequest,
                    new Response { IsSuccess = false, Message = Common.UserDoesNotExist,  });
        }

        private async Task<JwtSecurityToken> BuildJWTToken(User user)
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

        private async Task<string> UploadFile(byte[] bytes, string fileName)
        {
            string uploadsFolder = Path.Combine("Images", fileName);
            Stream stream = new MemoryStream(bytes);
            using (var ms = new FileStream(uploadsFolder, FileMode.Create))
            {
                await stream.CopyToAsync(ms);
            }
            return uploadsFolder;
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

            if (!string.IsNullOrWhiteSpace(registerUser.UserAvatar))
            {
                byte[] imgBytes = Convert.FromBase64String(registerUser.UserAvatar);
                string fileName = $"{Guid.NewGuid()}_{registerUser.FirstName.Trim()}_{registerUser.LastName.Trim()}.jpeg";
                string avatar = await UploadFile(imgBytes, fileName);
                registerUser.UserAvatar = avatar;
            }

            return user;
        }


        //[AllowAnonymous]
        //[HttpPost("RefreshToken")]
        //public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        //{
        //    var response = new Response();
        //    if (refreshTokenRequest is null)
        //    {
        //        response.IsSuccess = false;
        //        response.Message = "Invalid  request";
        //        return BadRequest(response);
        //    }

        //    var principal = GetPrincipalFromExpiredToken(refreshTokenRequest.AccessToken);

        //    if (principal != null)
        //    {
        //        var email = principal.Claims.FirstOrDefault(f => f.Type == ClaimTypes.Email);

        //        var user = await _userManager.FindByEmailAsync(email?.Value);

        //        if (user is null || user.RefreshToken != refreshTokenRequest.RefreshToken)
        //        {
        //            response.ErrorMessage = "Invalid Request";
        //            return BadRequest(response);
        //        }

        //        string newAccessToken = GenerateAccessToken(user);
        //        string refreshToken = GenerateRefreshToken();

        //        user.RefreshToken = refreshToken;
        //        await _userManager.UpdateAsync(user);

        //        response.IsSuccess = true;
        //        response.Content = new AuthenticationResponse
        //        {
        //            RefreshToken = refreshToken,
        //            AccessToken = newAccessToken
        //        };
        //        return Ok(response);
        //    }
        //    else
        //    {
        //        return null;//Response.ReturnErrorResponse("Invalid Token Found");
        //    }
        //}
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
