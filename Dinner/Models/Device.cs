using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinner.Models
{
    public class Device
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }

        public ICollection<Ticket> Tickets { get; set; }

        public Device()
        {
            Tickets = new List<Ticket>();
        }
    }
}