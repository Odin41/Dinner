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
        private static Logger logger = LogManager.GetCurrentClassLogger();

        IRepository repo;

        string UserName
        {
            get
            {
                return HttpContext?.User.Identity.Name;
            }
        }



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
                    return WriteError(Resources.Resource.RoomTableEmptyError, "HomeController", "Index");
                }
                ViewBag.rooms = repo.GetRooms();
                if (repo.GetDevices().Count() == 0)
                {
                   return WriteError(Resources.Resource.DeviceTableEmptyError, "HomeController", "Index");
                }
                ViewBag.devices = repo.GetDevices();

            }
            catch (InvalidOperationException ex)
            {
                return WriteError(string.Format(Resources.Resource.ErrorOnThePageMessage, @"Home\Index") + ex.Message, "HomeController", "Index");
            }

            catch (System.Data.Entity.Migrations.Infrastructure.AutomaticMigrationsDisabledException e)
            {
                return WriteError(string.Format(Resources.Resource.DataBaseErrorMessage,
                    Resources.Resource.DataBaseNeedMigrationMessage) + e.Message, "HomeController", "Index");
                
            }
            return View("Index");
        }


        //[HandleError(ExceptionType = typeof(Exception), View = "Error")]
        [HttpGet]
        private ActionResult WriteError(string message, string controllerName, string action)
        {
           logger.Error(message);
           //HandleErrorInfo ex = new HandleErrorInfo(new Exception(message), controllerName, action);
            // throw new Exception(message);
            //View("Error", ex);
           
            return new HttpStatusCodeResult(500);

        }

        /// <summary>
        /// Получение активного билета для пользователя
        /// </summary>
        /// <returns>Билет пользователя.</returns>
        public async Task<Ticket> GetTicketForUserAsync()
        {
            Ticket resultTicket = null;
            try
            {
                string userName = UserName;
                logger.Info("Проверка наличия билета для пользователя: " + userName + ".");
                ApplicationUser appUser = await repo.GetUserAsync(userName);
                logger.Info("Объект ApplicationUser успешно получен: " + userName + ".");
                
                if (appUser != null)
                {
                    resultTicket = await repo.GetUserTicketAsync(appUser);
                }
            }
            catch(Exception ex)
            {
                WriteError("Ошибка выполнения метода получения билета для пользователя." + ex.Message, "Home", "GetTicketForUser");
            }
            return resultTicket;
        }

        /// <summary>
        /// Получение статуса билета
        /// </summary>
        /// <returns>Сообщение стоит/не стоит в очереди пользователь. В случае если стоит указывается номер в очереди и куда занята очередь.</returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetTicketStatusAsync()
        {
            Ticket ticket = await GetTicketForUserAsync();
            string result = "";
            if (ticket != null)
            {
                int count = await GetTicketNumberInQueueAsync(ticket);
                ViewData["TicketNumber"] = count;
                ViewData["DeviceName"] = ticket.Device.Name;
                ViewData["RoomName"] = ticket.Device.Room.Name;

                result = string.Format(Resources.Resource.CheckQueueStatusFind, count, ticket.Device.Name, ticket.Device.Room.Name);
                logger.Info(result);
            }
            else
            {
                logger.Info("Объект Ticket для пользователя: " + UserName + " отсутствует.");
                result = Resources.Resource.CheckQueueStatusNotFind;
                logger.Info(result);
            }
           
            return PartialView("TicketInfo", ticket);
        }

        /// <summary>
        /// Получение номера в очереди к устройству
        /// </summary>
        /// <param name="ticket">Билет в очереди</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<int> GetTicketNumberInQueueAsync(Ticket ticket)
        {
            int count = -1;
            try
            {
                if (ticket == null)
                {
                    ticket = await GetTicketForUserAsync();
                }
                string userName = UserName;
                logger.Info("Получение номера в очереди для пользователя: " + userName + ".");
                count = await repo.GetNumberInQueueAsync(ticket);
                logger.Info("Номер пользователя: " + userName + " в очереди успешно получен.");
                if (count < 1)
                {
                    logger.Warn("Номер пользователя в очереди меньше 1.");
                }
            }
            catch (Exception ex)
            {
                WriteError("Ошибка выполнения метода получение номера в очереди к устройству." + ex.Message, "Home", "GetTicketNumberInQueue");
            }
            return count;
        }

        /// <summary>
        /// Встать в очередь к любому устройству
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> TakeAnyQueueAsync()
        {
            logger.Info("Пользователь: " + UserName + " хочет занять очередь к любому устройству.");
            return await TakeQueueAsync(-1, -1);
        }

        /// <summary>
        /// Встать в очередь к любому устройству в конкретной комнате комнате
        /// </summary>
        /// <param name="roomId">Идентификатор комнаты в базе</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> TakeQueueToRoomAsync(int roomId)
        {
            
            if (roomId < 1)
            {
                logger.Warn("Пользователь: " + UserName + " хочет занять очередь к любому устройству в комнате с идентификатором [ " + roomId + " ]");
                return null;
            }
            logger.Info("Пользователь: " + UserName + " хочет занять очередь к любому устройству в комнате с идентификатором [ " + roomId + " ]");
            return await TakeQueueAsync(roomId, -1);
        }

        /// <summary>
        /// Встать в очередь к конкретному устройству
        /// </summary>
        /// <param name="deviceid">Идентификатор устройства в базе</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> TakeQueueToDeviceAsync(int deviceid)
        {
           
            if (deviceid < 1)
            {
                logger.Warn("Пользователь: " + UserName + " хочет занять очередь к устройству с идентификатором [ " + deviceid + " ]");
                return null;
            }
            logger.Info("Пользователь: " + UserName + " хочет занять очередь к устройству с идентификатором [ " + deviceid + " ]");
            return await TakeQueueAsync(-1, deviceid);
        }

        private async Task<ActionResult> TakeQueueAsync(int roomId, int deviceId)
        {
            Ticket ticket = null;
            try
            {
                ticket = await GetTicketForUserAsync();

                if (ticket == null)
                {
                    int device = -1;
                    //Встать в любую очередь 
                    if (roomId < 0 && deviceId < 0)
                    {
                        logger.Info("Получение идентификатора устройства с минимальным количеством открытых билетов.");
                        device = await repo.GetDeviceIdAsync();
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
                        device = await repo.GetDeviceIdByRoomAsync(roomId);
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
                        if (deviceId < 0)
                        {
                            logger.Error("Номер устройства не может быть меньше 1.");
                            return RedirectToAction("Index");
                        }

                        if (await repo.CheckDeviceAsync(deviceId))
                        {
                            device = deviceId;
                        }
                    }

                    if (device > 0)
                    {
                        string userName = UserName;
                        logger.Error("Подготовка к записи в базу данных нового билета для пользователя: " + userName + ".");
                        logger.Info("Получение объекта ApplicationUser для учетной записи: " + userName + ".");
                        ApplicationUser appUser = await repo.GetUserAsync(userName);
                        DateTime createTime = DateTime.Now;

                        logger.Info("Создание билета с параметрами : DeviceId=" + device + ", UserId=" + appUser.Id + ", CreateTime=" + createTime + " .");
                        Ticket newTicket = new Ticket()
                        {
                            DeviceId = device,
                            UserId = appUser.Id,
                            CreateTime = DateTime.Now
                        };


                        logger.Info("Создание нового билета в базе.");
                        repo.CreateTicket(newTicket);
                        repo.Save();

                        ticket = newTicket;
                    }
                    return PartialView("QueueInfo", ticket);
                }
                return null;
            }
            catch (Exception e)
            {
                return WriteError(e.Message, "HomeController", "TakeQueue");
            }
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
            //logger.Info("Освобождение контролера HomeController.");
            if (disposing)
            {
                //logger.Info("Освобождение объекта IRepository.");
                repo.Dispose();
                //logger.Info("Освобождение объекта IRepository завершено.");
            }
            base.Dispose(disposing);
            //logger.Info("Освобождение контролера HomeController завершено.");
        }

    }
}
