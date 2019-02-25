using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Dinner.Models
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser> 
    {
        public ApplicationContext() : base("DinnerContext") {

            Database.SetInitializer(new ApplicationDBInitializer());
        }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }

        public DbSet<Room> Rooms {get;set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
    }

    public class ApplicationDBInitializer : CreateDatabaseIfNotExists<ApplicationContext>
    {
        protected override void Seed(ApplicationContext context)
        {
            Room r1 = new Room { Id = 1, Name = "Комната 1" };
            Room r2 = new Room { Id = 2, Name = "Комната 2" };

            context.Rooms.Add(r1);
            context.Rooms.Add(r2);

            context.Devices.Add(new Device { Name = "Микроволновка №1", Room = r1 });
            context.Devices.Add(new Device { Name = "Микроволновка №2", Room = r1 });
            context.Devices.Add(new Device { Name = "Микроволновка №1", Room = r2 });
            context.Devices.Add(new Device { Name = "Микроволновка №2", Room = r2 });

            base.Seed(context);
        }
    }
}