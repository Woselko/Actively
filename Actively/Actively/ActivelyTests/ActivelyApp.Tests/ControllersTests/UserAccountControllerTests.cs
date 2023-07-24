using ActivelyApp.Controllers;
using ActivelyApp.Controllers.Authentication;
using ActivelyApp.Models.Authentication.Password;
using ActivelyApp.Services.UserServices.EmailService;
using ActivelyDomain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private Mock<UserManager<User>>? _userManagerMock;
        private Mock<SignInManager<User>>? _signInManagerMock;
        private Mock<RoleManager<IdentityRole>>? _roleManagerMock;
        private UserAccountController? _controller;

        public UserAccountControllerTests()
        {
            var roleStore = new Mock<IRoleStore<IdentityRole>>();
            _configurationMock = new Mock<IConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
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
            User user = null;
            if (userExist)
            {
                _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User()
                {
                    Id = "UlalalaJakOnJaTralalala",
                    UserName = "TestUser",
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

        [Theory]
        [InlineData(true, "OldPassword1!", "NewPassword1!", "NewPassword1!")] // positive scenario
        [InlineData(false, "OldPassword1!", "NewPassword1!", "NewPassword1!")]// negative scenario
        [InlineData(true, "OldPassword1!", "passwordDoNotMatch1!", "NewPassword1!")] // negative scenario
        public void ChangePassword_ReturnsProperResponse_WhenDataIsGiven(bool userExist, string oldPswrd, string newPswrd, string confirmPswrd)
        {
            //arrange
            User user = null;
            if (userExist)
            {
                _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(new User()
                {
                    Id = "UlalalaJakOnJaTralalala",
                    UserName = "TestUser",
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
                                        new Claim(ClaimTypes.Name, "Test@Email.com")
            }, "TestAuthentication"));

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext { User = userClaims };

            if(newPswrd == confirmPswrd && userExist)
            {
                _userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success);
            }
            else
            {
                _userManagerMock.Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Failed());
            }
            var changePswtdDto = new ChangePassword()
            {
                OldPassword = oldPswrd,
                NewPassword = newPswrd,
                ConfirmPassword = confirmPswrd,
            };

            var result = _controller.ChangePassword(changePswtdDto).Result as ObjectResult;

            //assert
            if (newPswrd == confirmPswrd && userExist)
                Assert.Equal(StatusCodes.Status200OK, result?.StatusCode);
            else
            {
                if(!userExist)
                    Assert.Equal(StatusCodes.Status403Forbidden, result?.StatusCode);
                else
                    Assert.Equal(StatusCodes.Status500InternalServerError, result?.StatusCode);
            }
                
        }
    }
}
