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

        private ActionResult ShowErrorPage(string message, string controllerName, string action)
        {
            logger.Error(message);
            return View("Error", new HandleErrorInfo(new Exception(message), controllerName, action));
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

            logger.Info("Получение формы диалога авторизации по переходу со страницы " + returnUrl + ".");
            
            ViewBag.returnUrl = returnUrl;
            return View("Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                logger.Info("Авторизация пользователя: " + model.Email + ".");
                logger.Info("Поиск пользователя: " + model.Email + " в базе.");
                ApplicationUser user = await UserManager.FindAsync(model.Email, model.Password);
                if (user == null)
                {
                    logger.Info("Пользователь: " + model.Email + " в базе не найден.");
                    ModelState.AddModelError("", Resources.Resource.AuthenticationError);
                }
                else
                {
                    logger.Info("Пользователь: " + model.Email + " в базе успешно найден.");
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    logger.Info("Производим выход из системы предыдущего пользователя.");

                    AuthenticationManager.SignOut();
                    logger.Info("Производим вход в систему пользователя " + model.Email + ".");
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);

                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        logger.Info("Производим переход на домашнюю страницу.");
                        return RedirectToAction("Index", "Home");
                    }
                    logger.Info("Производим переход на страницу с которой был запрос на авторизацию.");
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            logger.Info("Отображение формы диалога авторизации по переходу со страницы " + returnUrl + ".");
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
                logger.Info("Регистрация пользователя: " + model.Email + ".");
                logger.Info("Создание ApplicationUser нового пользователя: " + model.Email + ".");
                ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                logger.Info("Создание ApplicationUser нового пользователя: " + model.Email + " успешно.");
                logger.Info("Запись нового пользователя: " + model.Email + " в базу.");
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    logger.Info("Запись нового пользователя: " + model.Email + " в базу успешно.");
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    logger.Error("Запись нового пользователя: " + model.Email + " в базу завершилось с ошибкой.");
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                        logger.Error(error);
                    }
                }
            }
            logger.Info("Отображение формы диалога регистрации.");
            return View(model);
        }

        public ActionResult Register()
        {
            logger.Info("Отображение формы диалога регистрации.");
            return View("Register");
        }
    }
}