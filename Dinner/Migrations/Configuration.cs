namespace Dinner.Migrations
{
    using Dinner.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Dinner.Models.ApplicationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Dinner.Models.ApplicationContext";
        }

        protected override void Seed(Dinner.Models.ApplicationContext context)
        {
            //Room r1 = new Room { Id = 1, Name = "������� 1" };
            //Room r2 = new Room { Id = 2, Name = "������� 2" };

            //context.Rooms.Add(r1);
            //context.Rooms.Add(r2);

            //context.Devices.Add(new Device { Name = "������������� �1", Room = r1 });
            //context.Devices.Add(new Device { Name = "������������� �2", Room = r1 });
            //context.Devices.Add(new Device { Name = "������������� �1", Room = r2 });
            //context.Devices.Add(new Device { Name = "������������� �2", Room = r2 });
        }
    }
}
