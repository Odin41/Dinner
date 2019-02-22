using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dinner.Controllers;
using Dinner.Models;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace Dinner.Tests.Controllers
{
    public class AccountControllerTest
    {
        AccountController controller = new AccountController();



        [Fact]
        public void RegisterNotNullView()
        {
            
            ViewResult result = controller.Register() as ViewResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void RegisterViewEqualsRegisterChtml()
        {
            AccountController controller = new AccountController();

            ViewResult result = controller.Register() as ViewResult;

            Assert.Equal("Register", result.ViewName);
        }

        string url = "TestUrl";

        [Fact]
        public void LoginNotNullView()
        {
            AccountController controller = new AccountController();
            ViewResult result = controller.Login(url) as ViewResult;

            Assert.Equal(result.ViewBag.returnUrl, url);
            Assert.NotNull(result);
        }
        
        [Fact]
        public void LoginViewEqualsLoginChtml()
        {
            AccountController controller = new AccountController();

            ViewResult result = controller.Login(url) as ViewResult;

            Assert.Equal("Login", result.ViewName);
        }


        [Fact]
        public void RegisterView()
        {
            // Arrange
            //HttpContext.Current = CreateHttpContext(userLoggedIn: true);
            //var userStore = new Mock<IUserStore<ApplicationUser>>();
            //var userManager = new Mock<ApplicationUserManager>(userStore.Object);
            //var authenticationManager = new Mock<IAuthenticationManager>();
            //var signInManager = new Mock<ApplicationSignInManager>(userManager.Object, authenticationManager.Object);

            //var accountController = new AccountController(
            //    userManager.Object, signInManager.Object, authenticationManager.Object);

            //// Act
            //var result = accountController.Register();

            //// Assert
            //Assert.That(result, Is.TypeOf<RedirectToRouteResult>());
        }




    }
}
