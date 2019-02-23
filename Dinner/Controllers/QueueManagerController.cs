using Dinner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Dinner.Filters;
using NLog;

namespace Dinner.Controllers
{
    [Culture]
    public class QueueManagerController : Controller
    {
        ApplicationContext db = new ApplicationContext();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        IRepository repo;

        public QueueManagerController(IRepository r)
        {
            repo = r;
        }

        public QueueManagerController()
        {
            repo = new SQLRepository();
        }


        [HttpGet]
        public ActionResult Index()
        {
            logger.Info("Получение страницы управления билетами.");
            var tickets = repo.GetAllOpenTickets();

            ViewBag.userName = HttpContext.User.Identity.Name;

            logger.Info("Отображение страницы со сформированной таблицей.");
            return View("Index", tickets.ToList());
        }

        [HttpGet]
        public async Task<ActionResult> CloseTicket(int id)
        {
            logger.Info("Закрытие билета.");
            int x = await repo.CloseTicketAsync(id);
            repo.Save();
            return Index();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}