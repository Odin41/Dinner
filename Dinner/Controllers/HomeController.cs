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
        public async Task<ActionResult> Index()
        {
            try
            {
                string userName = HttpContext?.User.Identity.Name;
                logger.Info(string.Format(Resources.Resource.GetPageLogMessage, @"Home\Index", userName));
                ViewBag.userName = userName;
                ApplicationUser appUser = await repo.GetUserAsync(userName);
                if (appUser == null)
                {
                    logger.Error(string.Format(Resources.Resource.UserNotFoundError, userName));
                    return RedirectToAction("Logout", "Account");
                }

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

        private ActionResult RedirectToLogout()
        {
            return RedirectToAction("Logout", "Account");
        }

        private ActionResult ShowErrorPage(string message, string controllerName, string action)
        {
            logger.Error(message);
            return View("Error", new HandleErrorInfo(new Exception(message), controllerName, action));
        }

        /// <summary>
        /// Получение активного билета для пользователя
        /// </summary>
        /// <returns>Сообщение стоит/не стоит в очереди пользователь. В случае если стоит указывается куда занята очередь.</returns>
        private async Task<Ticket> GetTicketForUser()
        {
            string userName = HttpContext.User.Identity.Name;
            logger.Info("Проверка наличия билета для пользователя: " + userName + ".");
            logger.Info("Получение объекта ApplicationUser для учетной записи: " + userName + ".");
            ApplicationUser appUser = await repo.GetUserAsync(userName);
            logger.Info("Объект ApplicationUser успешно получен: " + userName + ".");
            Ticket resultTicket = null;
            if (appUser != null)
            {
                logger.Info("Получение объекта Ticket для пользователя: " + userName + ".");
                resultTicket = await repo.GetUserTicketAsync(appUser);

                if (resultTicket != null)
                {
                    logger.Info("Объект Ticket для пользователя: " + userName + " успешно получен.");
                }
                else
                {
                    logger.Info("Объект Ticket для пользователя: " + userName + " отсутствует.");
                }
            }
            return resultTicket;
        }

        /// <summary>
        /// Получение статуса билета
        /// </summary>
        /// <returns>Сообщение стоит/не стоит в очереди пользователь. В случае если стоит указывается номер в очереди и куда занята очередь.</returns>
        [Authorize]
        [HttpGet]
        public async Task<string> GetTicketStatus()
        {
            Ticket ticket = await GetTicketForUser();
            string userName = HttpContext.User.Identity.Name;
            string result = "";
            if (ticket != null)
            {
                int count = await GetTicketNumberInQueue(ticket);
                result = string.Format(Resources.Resource.CheckQueueStatusFind, count, ticket.Device.Name, ticket.Device.Room.Name);
                logger.Info(result);
            }
            else
            {
                logger.Info("Объект Ticket для пользователя: " + userName + " отсутствует.");
                result = Resources.Resource.CheckQueueStatusNotFind;
                logger.Info(result);
            }
            return result;
        }

        /// <summary>
        /// Получение номера в очереди к устройству
        /// </summary>
        /// <param name="ticket">Билет в очереди</param>
        /// <returns></returns>
        private async Task<int> GetTicketNumberInQueue(Ticket ticket)
        {
            if (ticket == null)
            {
                ticket = await GetTicketForUser();
            }
            string userName = HttpContext.User.Identity.Name;
            logger.Info("Получение номера в очереди для пользователя: " + userName + ".");
            int count = await repo.GetNumberInQueueAsync(ticket);
            logger.Info("Номер пользователя: " + userName + " в очереди успешно получен.");
            if (count < 1)
            {
                logger.Warn("Номер пользователя в очереди меньше 1.");
            }
            return count;
        }

        /// <summary>
        /// Встать в очередь к любому устройству
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<string> TakeAnyQueue()
        {
            string userName = HttpContext.User.Identity.Name;
            logger.Info("Пользователь: " + userName + " хочет занять очередь к любому устройству.");
            return await TakeQueue(-1, -1);
        }

        /// <summary>
        /// Встать в очередь к любому устройству в конкретной комнате комнате
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты в базе</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<string> TakeQueueToRoom(int roomId)
        {
            string userName = HttpContext.User.Identity.Name;
            logger.Info("Пользователь: " + userName + " хочет занять очередь к любому устройству в комнате с идентификатором [ " + roomId + " ]");
            return await TakeQueue(roomId, -1);
        }

        /// <summary>
        /// Встать в очередь к конкретному устройству
        /// </summary>
        /// <param name="deviceid">Идентификатор устройства в базе</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<string> TakeQueueToDevice(int deviceid)
        {
            string userName = HttpContext.User.Identity.Name;
            logger.Info("Пользователь: " + userName + " хочет занять очередь к устройству с идентификатором [ " + deviceid + " ]");
            return await TakeQueue(-1, deviceid);
        }

        [Authorize]
        [HttpPost]
        public async Task<string> TakeQueue(int roomId, int deviceId)
        {
            string userName = HttpContext.User.Identity.Name;
            Ticket ticket = await GetTicketForUser();
            if (ticket == null)
            {
                int device = -1;
                //Встать в любую очередь 
                if (roomId < 0 && deviceId < 0)
                {
                    logger.Info("Получение идентификатора устройства с минимальным количеством открытых билетов.");
                    device = repo.GetDeviceId();
                    if (device > 0)
                    {
                        logger.Info("Устройства с минимальным количеством открытых билетов успешно получено. Номер устройства [ " + deviceId + " ] .");
                    }
                    else
                    {
                        logger.Error("Ошибка получения устройства с минимальным количеством открытых билетов.");
                    }
                }

                //Встать в очередь к любому устройству в комнате
                if (roomId > 0 && deviceId < 0)
                {
                    logger.Info("Получение идентификатора устройства с минимальным количеством открытых билетов в комнате [ " + roomId + " ]");
                    device = repo.GetDeviceIdByRoom(roomId);
                    if (device > 0)
                    {
                        logger.Info("Устройства с минимальным количеством открытых билетов в комнате [ " + roomId + " ] успешно получено. Номер устройства [ " + deviceId + " ] .");
                    }
                    else
                    {
                        logger.Error("Ошибка получения устройства с минимальным количеством открытых билетов для комнаты [ " + roomId + " ].");
                    }
                }

                //Встать в очередь к определенному устройству в комнате
                if (roomId < 0 && deviceId > 0)
                {
                    if (device > 0)
                    {
                        device = deviceId;
                    }
                    else
                    {
                        logger.Error("Номер устройства не может быть меньше 1.");
                        RedirectToAction("Index");
                    }
                }

                if (device > 0)
                {
                    logger.Error("Подготовка к записи в базу данных нового билета для пользователя: " + userName + ".");
                    logger.Info("Получение объекта ApplicationUser для учетной записи: " + userName + ".");
                    ApplicationUser appUser = await repo.GetUserAsync(userName);
                    DateTime createTime = DateTime.Now;

                    logger.Info("Создание билета с параметрами : DeviceId=" + device + ", UserId="+ appUser.Id+", CreateTime="+ createTime + " .");
                    Ticket createdTicket = new Ticket()
                    {
                        DeviceId = device,
                        UserId = appUser.Id,
                        CreateTime = DateTime.Now
                    };

                    try
                    {
                        logger.Info("Сохранние билета в базу.");
                        repo.CreateTicket(createdTicket);
                        logger.Info("Билет успешно сохранен в базу.");
                    }
                    catch (Exception e)
                    {
                        ShowErrorPage("Ошибка сохранние билета в базу.", "HomeController", "TakeQueue");
                    }
                }
            }


            return await GetTicketStatus(); 
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
            logger.Info("Освобождение контролера HomeController.");
            if (disposing)
            {
                logger.Info("Освобождение объекта IRepository.");
                repo.Dispose();
                logger.Info("Освобождение объекта IRepository завершено.");
            }
            base.Dispose(disposing);
            logger.Info("Освобождение контролера HomeController завершено.");
        }

    }
}
