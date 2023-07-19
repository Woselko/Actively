using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json.Linq;
using System.Text;
using Xunit;
using ActivelyApp.Controllers.Authentication;
using ActivelyApp.Services.UserServices.EmailService;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ActivelyApp.Models.Authentication.Registration;

namespace ActivelyApp.Tests.ControllersTests
{
    public class AuthenticationControllerTests
    {
        private Mock<IConfiguration>? _configurationMock;
        private Mock<IEmailService>? _emailServiceMock;
        private Mock<UserManager<IdentityUser>>? _userManagerMock;
        private Mock<SignInManager<IdentityUser>>? _signInManagerMock;
        private Mock<RoleManager<IdentityRole>>? _roleManagerMock;
        private AuthenticationController? _controller;

        public AuthenticationControllerTests()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _configurationMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<IdentityUser>>(_userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(), null, null, null, null);
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
            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser("TestUser")
            {
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString(),
                EmailConfirmed = false,
            });
            if (shouldSuccess)
                _userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            else
                _userManagerMock.Setup(x => x.ConfirmEmailAsync(It.IsAny<IdentityUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed());

            //act
            var result = _controller.ConfirmEmail(token, email).Result as ObjectResult;

            //assert
            if (shouldSuccess)
                Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
            else
                Assert.Equal(StatusCodes.Status500InternalServerError, result?.StatusCode);
        }

        [Theory]
        //[InlineData(false, true, true)] // positive scenario
        [InlineData(true, true, true)] // negative scenario
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

            if (userExist)
            {
                _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser("TestUser")
                {
                    Email = "test@test.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = false,
                });
            }
            else
            {
                _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).Returns(new Task<IdentityUser>(null));
            }

            var result = _controller.Register(model).Result as ObjectResult;

            if (userExist)
            {
                Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
            }
        }
    }
}