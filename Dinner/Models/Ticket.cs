using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dinner.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int DeviceId { get; set; }
        public Device Device { get; set; }
        

        public DateTime? CreateTime { get; set; }
        public DateTime? CloseTime { get; set; }
    }
}