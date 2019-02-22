using Dinner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Dinner.Filters;
using System.Threading;
using System.Globalization;
using NLog;
using Microsoft.AspNet.Identity.Owin;

namespace Dinner.Controllers
{
    [Culture]
    public class HomeController : Controller
    {
        ///ApplicationContext db = new ApplicationContext();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        IRepository repo;

        public HomeController()
        {
            repo = new SQLRepository();
        }


        public HomeController(IRepository r)
        {
            repo = r;
        }

        [Authorize]
        [HandleError(ExceptionType = typeof(NullReferenceException), View = "Error")]
        public ActionResult Index()
        {
            try
            {
                string userName = HttpContext?.User.Identity.Name;
                logger.Info(string.Format(Resources.Resource.GetPageLogMessage, @"Home\Index", userName));
                ViewBag.userName = userName;
                GetUserByName(userName);

                if (repo.GetRooms().Count() == 0)
                {
                    return ShowErrorPage(Resources.Resource.RoomTableEmptyError, "HomeController", "Index");
                }
                ViewBag.rooms = repo.GetRooms();
                if (repo.GetDevices().Count() == 0)
                {
                    return ShowErrorPage(Resources.Resource.DeviceTableEmptyError, "HomeController", "Index");
                }
                ViewBag.devices = repo.GetDevices();

            }
            catch (InvalidOperationException ex)
            {
                return ShowErrorPage(string.Format(Resources.Resource.ErrorOnThePageMessage, @"Home\Index") + ex.Message, "HomeController", "Index");
            }

            catch (System.Data.Entity.Migrations.Infrastructure.AutomaticMigrationsDisabledException e)
            {
                return ShowErrorPage(string.Format(Resources.Resource.DataBaseErrorMessage,
                    Resources.Resource.DataBaseNeedMigrationMessage) + e.Message, "HomeController", "Index");
            }
            return View("Index");
        }

        private ApplicationUser GetUserByName(string userName)
        {
            //ApplicationUser appUser = db.Users.FirstOrDefault(n => n.UserName == userName);
            ApplicationUser appUser = repo.GetUser(userName);
            if (string.IsNullOrEmpty(userName) || appUser == null)
            {
                logger.Info(string.Format(Resources.Resource.UserNotFoundError, userName));
                AccountController accountController = new AccountController();
                accountController.Logout();
            }
            return appUser;
        }

        private ActionResult ShowErrorPage(string message, string controllerName, string action)
        {
            logger.Error(message);
            return View("Error", new HandleErrorInfo(new Exception(message), controllerName, action));
        }

        [Authorize]
        [HttpGet]
        public string CheckQueueStatus()
        {
            string userName = HttpContext.User.Identity.Name;
            logger.Info("Получение информации о статусе в очереди пользователем: " + userName);

            ApplicationUser appUser = GetUserByName(userName);
            string result = "";
            if (appUser != null)
            {
                //Ticket ticket = db.Tickets.Include(t => t.Device).Include(d => d.Device.Room).FirstOrDefault(n => n.UserId == appUser.Id && n.CloseTime == null);
                Ticket ticket = repo.GetUserTicket(appUser);

                if (ticket != null)
                {
                    //int count = db.Tickets.Count(t => t.CloseTime == null && t.Id < ticket.Id && t.DeviceId == ticket.DeviceId) + 1;

                    int count = repo.GetNumberInQueue(ticket);

                    //ChangeLanguage();
                    result = string.Format(Resources.Resource.CheckQueueStatusFind, count, ticket.Device.Name, ticket.Device.Room.Name);
                }
                else
                {
                    result = Resources.Resource.CheckQueueStatusNotFind;
                }
            }
            return result;
        }

        //private void ChangeLanguage()
        //{
        //    string CurrentLanguageCode = HttpContext.Request.Cookies["lang"].Value;
        //    if (CurrentLanguageCode != null)
        //    {
        //        try
        //        {
        //            Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(CurrentLanguageCode);
        //        }
        //        catch (Exception)
        //        {
        //            throw new NotSupportedException($"Invalid language code '{CurrentLanguageCode}'.");
        //        }
        //    }
        //}

        [Authorize]
        [HttpPost]
        public string TakeAnyQueue()
        {
            return TakeQueue(-1, -1);
        }

        [Authorize]
        [HttpPost]
        public string TakeQueueToRoom(int roomId)
        {
            return TakeQueue(roomId, -1);
        }

        [Authorize]
        [HttpPost]
        public string TakeQueueToDevice(int deviceid)
        {
            return TakeQueue(-1, deviceid);
        }

        [Authorize]
        [HttpPost]
        public string TakeQueue(int roomId, int deviceId)
        {
            string userName = HttpContext.User.Identity.Name;
            ApplicationUser appUser = GetUserByName(userName);
            if (appUser != null)
            {
                //проверить очередь
                //Ticket ticket = db.Tickets.Include(t=>t.Device).Include(d => d.Device.Room).FirstOrDefault(n => n.UserId == appUser.Id && n.CloseTime == null);
                Ticket ticket = repo.GetUserTicket(appUser);
                if (ticket == null)
                {
                    int device = -1;
                    //create
                    //Встать в любую очередь
                    if (roomId < 0 && deviceId < 0)
                    {
                        //device = db.Devices.OrderBy(d => d.Tickets.Count).FirstOrDefault().Id;
                        device = repo.GetDeviceId();
                    }

                    //Встать в очередь к любому устройству в комнате
                    if (roomId > 0 && deviceId < 0)
                    {
                        //device = db.Devices.Where(d => d.Room.Id == roomId).OrderBy(d => d.Tickets.Count).FirstOrDefault().Id;
                        device = repo.GetDeviceIdByRoom(roomId);
                    }

                    //Встать в очередь к определенному устройству в комнате
                    if (roomId < 0 && deviceId > 0)
                    {
                        device = deviceId;
                    }

                    Ticket createdTicket = new Ticket()
                    {
                        DeviceId = device,
                        UserId = appUser.Id,
                        CreateTime = DateTime.Now
                    };


                    //db.Tickets.Add(createdTicket);
                    //db.SaveChanges();
                    repo.CreateTicket(createdTicket);
                }
            }

            return CheckQueueStatus(); 
        }


        public ActionResult ChangeCulture(string lang)
        {
            string returnUrl = Request.UrlReferrer.AbsolutePath;

            List<string> cultures = new List<string>() { "ru", "en" };
            if (!cultures.Contains(lang))
            {
                lang = "ru";
            }

            HttpCookie cookie = Request.Cookies["lang"];
            if (cookie != null)
            {
                cookie.Value = lang;
            }
            else
            {
                cookie = new HttpCookie("lang");
                cookie.HttpOnly = false;
                cookie.Value = lang;
                cookie.Expires = DateTime.Now.AddYears(1);

            }

            Response.Cookies.Add(cookie);
            return Redirect(returnUrl);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
                repo.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
