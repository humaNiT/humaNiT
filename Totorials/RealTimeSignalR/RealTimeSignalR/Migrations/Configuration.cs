namespace RealTimeSignalR.Migrations
{
    using RealTimeSignalR.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<RealTimeSignalR.Models.RealTimeSignalRContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(RealTimeSignalR.Models.RealTimeSignalRContext context)
        {
            context.Employees.AddOrUpdate(
                e => e.Name,
                new Employee { Name = "Jannen Siahaan", Email = "j.siahaan@live.com", Salary = 1 },
                new Employee { Name = "Tolhas Tuna Tumure", Email = "tumure@live.com", Salary = 1 },
                new Employee { Name = "Jim Wang", Email = "jim.wang@microsoft.com", Salary = 1 }
                );
        }
    }
}
