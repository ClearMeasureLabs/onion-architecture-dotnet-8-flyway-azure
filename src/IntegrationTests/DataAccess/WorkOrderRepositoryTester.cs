using System;
using System.Threading.Tasks;
using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Handlers;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;
using ProgrammingWithPalermo.ChurchBulletin.IntegrationTests;
using ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.DataAccess;
using Shouldly;

namespace IntegrationTests.DataAccess
{
    [TestFixture]
    public class WorkOrderRepositoryTester
    {
        [Test]
        public async Task ShouldGetWorkOrderByNumber()
        {
            new DatabaseTester().Clean();

            var creator = new Employee("1", "1", "1", "1");
            var order1 = new WorkOrder();
            order1.Creator = creator;
            order1.Number = "123";
            var order2 = new WorkOrder();
            order2.Creator = creator;
            order2.Number = "456";

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(creator);
                context.Add(order1);
                context.Add(order2);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);
            WorkOrder? order123 = await repository.GetWorkOrderAsync("123");
            WorkOrder? order456 = await repository.GetWorkOrderAsync("456");

            Assert.That(order123.Id, Is.EqualTo(order1.Id));
            Assert.That(order456.Id, Is.EqualTo(order2.Id));
        }

        [Test]
        public async Task ShouldSaveWorkOrder()
        {
            new DatabaseTester().Clean();

            var creator = new Employee("1", "1", "1", "1");
            var assignee = new Employee("2", "2", "2", "2");
            var order = new WorkOrder();
            order.Creator = creator;
            order.Assignee = assignee;
            order.Title = "foo";
            order.Description = "bar";
            order.RoomNumber = "123 a";
            order.ChangeStatus(WorkOrderStatus.InProgress);
            order.Number = "123";
            new DateTime(2000, 1, 1, 8, 0, 0);
            new DateTime(2000, 1, 1, 8, 0, 0);
            new DateTime(2000, 1, 1, 8, 0, 0);

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(creator);
                context.Add(assignee);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);
            await repository.SaveAsync(order);

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                var rehydratedWorkOrder = context.Set<WorkOrder>()
                    .Include(wo => wo.Creator)
                    .Include(wo => wo.Assignee)
                    .Single(wo => wo.Id == order.Id);
                Assert.That(rehydratedWorkOrder.Id, Is.EqualTo(order.Id));
                Assert.That(rehydratedWorkOrder.Creator!.Id, Is.EqualTo(order.Creator.Id));
                Assert.That(rehydratedWorkOrder.Assignee!.Id, Is.EqualTo(order.Assignee.Id));
                Assert.That(rehydratedWorkOrder.Title, Is.EqualTo(order.Title));
                Assert.That(rehydratedWorkOrder.Description, Is.EqualTo(order.Description));
                Assert.That(rehydratedWorkOrder.Status, Is.EqualTo(order.Status));
                Assert.That(rehydratedWorkOrder.RoomNumber, Is.EqualTo(order.RoomNumber));
                rehydratedWorkOrder.Number.ShouldBe(order.Number);
            }
        }
    }
}