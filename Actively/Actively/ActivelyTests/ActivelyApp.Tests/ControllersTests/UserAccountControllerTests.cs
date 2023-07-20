using ActivelyApp.Controllers;
using ActivelyApp.Controllers.Authentication;
using ActivelyApp.Services.UserServices.EmailService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.SqlServer.Management.Smo;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActivelyApp.Tests.ControllersTests
{
    public class UserAccountControllerTests
    {

        private Mock<IConfiguration>? _configurationMock;
        private Mock<IEmailService>? _emailServiceMock;
        private Mock<UserManager<IdentityUser>>? _userManagerMock;
        private Mock<SignInManager<IdentityUser>>? _signInManagerMock;
        private Mock<RoleManager<IdentityRole>>? _roleManagerMock;
        private UserAccountController? _controller;

        public UserAccountControllerTests()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _configurationMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();
            _userManagerMock = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<IdentityUser>>(_userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(), null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(roleStore.Object, null, null, null, null);

            _controller = new UserAccountController(_userManagerMock.Object, _roleManagerMock.Object, _emailServiceMock.Object,
                _signInManagerMock.Object, _configurationMock.Object);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetEmailTwoFactorAuthentication_ReturnsProperResponse_WhenDataIsGiven(bool userExist)
        {
            //arrange
            IdentityUser user = null;
            if (userExist)
            {
                _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new IdentityUser("TestUser")
                {
                    Id = "UlalalaJakOnJaTralalala",
                    Email = "Test@Email.com",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = false,
                });
            }
            else
            {
                _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            }

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] {
                                        new Claim(ClaimTypes.NameIdentifier, "SomeValueHere"),
                                        new Claim(ClaimTypes.Name, "gunnar@somecompany.com")
            }, "TestAuthentication"));
            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = userClaims };

            //act
            var result = _controller.SetEmailTwoFactorAuthentication(userExist).Result as ObjectResult;

            //assert
            if (userExist)
                Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
            else
                Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
        }
    }
}
