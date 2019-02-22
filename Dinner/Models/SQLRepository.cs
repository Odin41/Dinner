using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
     

namespace Dinner.Models
{
    public class SQLRepository : IRepository
    {
        private ApplicationContext db;

        public SQLRepository()
        {
            db = new ApplicationContext();
        }

        public void CreateTicket(Ticket ticket)
        {
            db.Tickets.Add(ticket);
            db.SaveChanges();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public int GetDeviceId()
        {
            return db.Devices.OrderBy(d => d.Tickets.Count).FirstOrDefault().Id;
        }

        public int GetDeviceIdByRoom(int id)
        {
            return db.Devices.Where(d => d.Room.Id == id).OrderBy(d => d.Tickets.Count).FirstOrDefault().Id;
        }

        public IEnumerable<Device> GetDevices()
        {
           return db.Devices.ToList();
        }

        public int GetNumberInQueue(Ticket ticket)
        {
            int count = db.Tickets.Count(t => t.CloseTime == null && t.Id < ticket.Id && t.DeviceId == ticket.DeviceId);
            return ++count;
        }

        public IEnumerable<Room> GetRooms()
        {
            return db.Rooms.ToList();
        }

        public ApplicationUser GetUser(string userName)
        {
            return db.Users.FirstOrDefault(n => n.UserName == userName);
        }

        public Ticket GetUserTicket(ApplicationUser user)
        {
            return db.Tickets.Include(t => t.Device).Include(d => d.Device.Room).FirstOrDefault(n => n.UserId == user.Id && n.CloseTime == null);
        }
    }
}