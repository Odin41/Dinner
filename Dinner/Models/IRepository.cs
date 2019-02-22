using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinner.Models
{
    public interface IRepository : IDisposable
    {
        ApplicationUser GetUser(string userName);
        Ticket GetUserTicket(ApplicationUser user);

        IEnumerable<Device> GetDevices();
        IEnumerable<Room> GetRooms();
        //IEnumerable<Device> GetTickets();
        
        void CreateTicket(Ticket ticket);
        int GetNumberInQueue(Ticket ticket);

        int GetDeviceId();
        int GetDeviceIdByRoom(int id);
    }
}