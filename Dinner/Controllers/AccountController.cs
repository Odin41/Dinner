using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Dinner.Filters;
using Dinner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NLog;


namespace Dinner.Controllers
{
    [Culture]
    public class AccountController : Controller
    {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private void ShowErrorPage(string message, string controllerName, string action)
        {
            logger.Error(message);
            View("Error", new HandleErrorInfo(new Exception(message), controllerName, action));
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                if (HttpContext == null)
                {
                    ShowErrorPage(Resources.Resource.AccountHttpContextError, "AccountController", "AuthenticationManager");
                    return null;
                }
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View("Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindAsync(model.Email, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", Resources.Resource.AuthenticationError);
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    return Redirect(returnUrl);

                }

            }

            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        public ActionResult Logout()
        {
            if (AuthenticationManager != null)
            {
                logger.Info(string.Format(Resources.Resource.AccountExitMessage, AuthenticationManager.User.Identity.Name));
                AuthenticationManager.SignOut();
            }
            return RedirectToAction("Login");
        }


        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        public ActionResult Register()
        {
            return View("Register");
        }
    }
}