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
           
        }
    }
}
