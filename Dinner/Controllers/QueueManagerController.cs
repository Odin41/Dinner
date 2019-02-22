using Dinner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Dinner.Filters;

namespace Dinner.Controllers
{
    [Culture]
    public class QueueManagerController : Controller
    {
        ApplicationContext db = new ApplicationContext();

        [HttpGet]
        public ActionResult Index()
        {
            var tickets = db.Tickets.Include(t=>t.Device)
                                    .Include(d=>d.Device.Room)
                                    .Include(u => u.User)
                               .Where(t => t.CloseTime == null);

            ViewBag.userName = HttpContext.User.Identity.Name;

            return View("Index", tickets.ToArray());
        }

        [HttpGet]
        public ActionResult CloseTicket(int id)
        {
            db.Tickets.Find(id).CloseTime = DateTime.Now;
            db.SaveChanges();
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