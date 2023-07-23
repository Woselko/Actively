using Microsoft.AspNetCore.Identity;
using Xunit;
using ActivelyApp.Controllers.Authentication;
using ActivelyApp.Services.UserServices.EmailService;
using Microsoft.Extensions.Configuration;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ActivelyApp.Models.Authentication.Registration;
using ActivelyApp.Models.Authentication.Login;
using ActivelyDomain.Entities;

namespace ActivelyApp.Tests.ControllersTests
{
    public class AuthenticationControllerTests
    {
        private Mock<IConfiguration>? _configurationMock;
        private Mock<IEmailService>? _emailServiceMock;
        private Mock<UserManager<User>>? _userManagerMock;
        private Mock<SignInManager<User>>? _signInManagerMock;
        private Mock<RoleManager<IdentityRole>>? _roleManagerMock;
        private AuthenticationController? _controller;

        public AuthenticationControllerTests()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _configurationMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), 
                Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _controller = new AuthenticationController(_userManagerMock.Object, _roleManagerMock.Object,
                 _emailServiceMock.Object, _signInManagerMock.Object, _configurationMock.Object);  
        }

        [Theory]
        [InlineData("validToken", "test@test.com", true)]
        [InlineData("invalidToken", "test@test.com", false)]
        public void ConfirmEmail_ReturnsProperResponse_WhenDataIsGiven(string token, string email, bool shouldSuccess)
        {
            //arrange
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User()
            {
                UserName = "TestUser",
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
            });
            if (shouldSuccess)
                _userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            else
                _userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            //act
            var result = _controller.ConfirmEmail(token, email).Result as ObjectResult;

            //assert
            if (shouldSuccess)
                Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
            else
                Assert.Equal(StatusCodes.Status500InternalServerError, result?.StatusCode);
        }

        [Theory]
        [InlineData(true, true, true)] // negative scenario
        [InlineData(false, false, true)] // negative scenario
        [InlineData(false, true, false)] // negative scenario
        public void RegisterUser_ReturnsProperStatusCode_WhenDataProvided(bool userExist, bool roleExist, bool creationSucces)
        {
            //arrange
            RegisterUser model = new RegisterUser
            {
                Email = "test@test.com",
                Username = "Test",
                ConfirmPassword = "Test",
                Password = "Test",
            };
            var createdUser = new User()
            {
                UserName = "TestUser",
                Email = "test@test.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
            };

            _userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<User>())).ReturnsAsync("token");

            if (userExist)
            {
                _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(createdUser);
            }
            else
            {
                User user = null;
                _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            }

            if (roleExist)
            {
                _roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);
            }
            else
                _roleManagerMock.Setup(r => r.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(false);


            if (creationSucces)
            {
                _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            }
            else 
            {
                _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());
            }
                

            //act
            var result = _controller.Register(model).Result as ObjectResult;

            //assert
            if (userExist)
            {
                Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
            }
            else
            {
                if(!roleExist) 
                {
                    Assert.Equal(StatusCodes.Status500InternalServerError, result?.StatusCode);
                }
                else
                {
                    if (creationSucces)
                    {
                        Assert.Equal(StatusCodes.Status201Created, result?.StatusCode);
                    }
                    else
                    {
                        Assert.Equal(StatusCodes.Status500InternalServerError, result?.StatusCode);
                    }
                }
            }
        }

        [Theory]
        [InlineData(true, true, true, true)]
        [InlineData(true, false, true, true)]
        [InlineData(false, false, true, true)]
        [InlineData(true, false, true, false)]
        [InlineData(false, false, false, false)]
        [InlineData(true, false, false, true)]
        public void Login_ReturnsProperStatusCode_WhenDataProvided(bool userExist, bool twoFactorAuthEnabled, bool correctPass, bool emailConfirmed)
        {
            //arrange
            LoginModel model = new LoginModel
            {
                UserName = "TestUserName",
                Password = "Test",
            };

            var createdUser = new User()
            {
                UserName = "TestUserName",
                Email = "test@test.com",
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = emailConfirmed,
                TwoFactorEnabled = twoFactorAuthEnabled,
            };
            _userManagerMock.Setup(x => x.GenerateTwoFactorTokenAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync("token");
            if (userExist)
            {
                _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(createdUser);
            }
            else
            {
                User user = null;
                _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            }

            _userManagerMock.Setup(x => x.CheckPasswordAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(correctPass);
            _userManagerMock.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<User>())).ReturnsAsync(emailConfirmed);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string>()
            {
                "Admin"
            });
            _configurationMock.Setup(x => x[It.IsAny<string>()]).Returns("this is my custom JWT Symmetric key for authentication");

            //act
            var result = _controller.Login(model).Result as ObjectResult;
            
            //assert
            if (userExist && correctPass && emailConfirmed)
            {
                Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
            }
            else
            {
                Assert.Equal(StatusCodes.Status401Unauthorized, result?.StatusCode);
            }
        }
    }
}