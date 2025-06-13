using Core.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;
using ProgrammingWithPalermo.ChurchBulletin.IntegrationTests;
using ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.DataAccess;
using System;

namespace IntegrationTests.DataAccess
{
    [TestFixture, Explicit]
    public class ZDataLoader
    {
        [Test, Category("DataLoader")]
        public void PopulateDatabase()
        {
            new DatabaseTester().Clean();
            var lead = new Role("Facility Lead", true, false);
            var fulfillment = new Role("Fulfillment", false, true);
            var db = TestHost.GetRequiredService<DbContext>();
            db.Add(lead);
            db.Add(fulfillment);
            db.SaveChanges();
            
            //Trainer1
            var jpalermo = new Employee("jpalermo", "Jeffrey", "Palermo", "jeffreypalermo@yahoo.com");
            jpalermo.AddRole(lead);
            jpalermo.AddRole(fulfillment);
            db.Add(jpalermo);

            //Person 1

            //Person 2
            
            //Person 3

            //Person 4
 
            //Person 5

            //Person 6

            //Person 7

            //Person 8
            
            //Person 9

            //Person 10

            //Person 11

            //Person 12

            //Person 13

            var hsimpson = new Employee("hsimpson", "Homer", "Simpson", "homer@simpson.com");
            hsimpson.AddRole(fulfillment);
            db.Add(hsimpson);

            foreach (WorkOrderStatus status in WorkOrderStatus.GetAllItems())
            {
                var order = new WorkOrder();
                order.Number = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
                order.Creator = jpalermo;
                order.Assignee = jpalermo;
                order.Status = status;
                order.Title = "Work Order starting in status " + status;
                order.Description = "Foo, foo, foo, foo " + status;
                order.CreatedDate = new DateTime(2000, 1, 1, 8, 0, 0);
                order.CompletedDate = new DateTime(2000, 1, 1, 8, 0, 0);
                order.ChangeStatus(WorkOrderStatus.Draft);
                order.ChangeStatus(WorkOrderStatus.Assigned);
                order.ChangeStatus(WorkOrderStatus.InProgress);
                order.ChangeStatus(WorkOrderStatus.Complete);

                db.Add(order);
            }

            var order2 = new WorkOrder();
            order2.Number = Guid.NewGuid().ToString().Substring(0, 5).ToUpper();
            order2.Creator = jpalermo;
            order2.Assignee = jpalermo;
            order2.Status = WorkOrderStatus.Complete;
            order2.Title = "Work Order starting in status ";
            order2.Description = "Foo, foo, foo, foo ";
            order2.CreatedDate = new DateTime(2000, 1, 1, 8, 0, 0);
            order2.CompletedDate = new DateTime(2000, 1, 1, 8, 0, 0);
            db.Add(order2);

            db.SaveChanges();
            db.Dispose();
        }
    }
}