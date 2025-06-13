using System;
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
        public void ShouldGetWorkOrderByNumber()
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
            WorkOrder? order123 = repository.GetWorkOrder("123");
            WorkOrder? order456 = repository.GetWorkOrder("456");

            Assert.That(order123.Id, Is.EqualTo(order1.Id));
            Assert.That(order456.Id, Is.EqualTo(order2.Id));
        }

        [Test]
        public void ShouldSaveWorkOrder()
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
            // These seem to be unused DateTime objects
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
            repository.Save(order);

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

        [Test]
        public void ShouldSearchBySpecificationWithAssignee()
        {
            new DatabaseTester().Clean();

            var employee1 = new Employee("1", "1", "1", "1");
            var employee2 = new Employee("2", "2", "2", "2");
            var order1 = new WorkOrder();
            order1.Creator = employee2;
            order1.Assignee = employee1;
            order1.Number = "123";
            var order2 = new WorkOrder();
            order2.Creator = employee1;
            order2.Assignee = employee2;
            order2.Number = "456";

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(employee1);
                context.Add(employee2);
                context.Add(order1);
                context.Add(order2);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);
            var specification = new WorkOrderSearchSpecification();
            specification.MatchAssignee(employee1);
            WorkOrder[] orders = repository.GetWorkOrders(specification);

            Assert.That(orders.Length, Is.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(order1.Id));
        }

        [Test]
        public void ShouldSearchBySpecificationWithCreator()
        {
            new DatabaseTester().Clean();

            var creator1 = new Employee("1", "1", "1", "1");
            var creator2 = new Employee("2", "2", "2", "2");
            var order1 = new WorkOrder();
            order1.Creator = creator1;
            order1.Number = "123";
            var order2 = new WorkOrder();
            order2.Creator = creator2;
            order2.Number = "456";

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(creator1);
                context.Add(creator2);
                context.Add(order1);
                context.Add(order2);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);
            var specification = new WorkOrderSearchSpecification();
            specification.MatchCreator(creator1);
            WorkOrder[] orders = repository.GetWorkOrders(specification);

            Assert.That(orders.Length, Is.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(order1.Id));
        }

        [Test]
        public void ShouldSearchBySpecificationWithFullSpecification()
        {
            new DatabaseTester().Clean();

            var employee1 = new Employee("1", "1", "1", "1");
            var employee2 = new Employee("2", "2", "2", "2");
            var order1 = new WorkOrder();
            order1.Creator = employee2;
            order1.Assignee = employee1;
            order1.Number = "123";
            order1.Status = WorkOrderStatus.Assigned;
            var order2 = new WorkOrder();
            order2.Creator = employee1;
            order2.Assignee = employee2;
            order2.Number = "456";
            order2.Status = WorkOrderStatus.Draft;

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(employee1);
                context.Add(employee2);
                context.Add(order1);
                context.Add(order2);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);
            var specification = new WorkOrderSearchSpecification();
            specification.MatchStatus(WorkOrderStatus.Assigned);
            specification.MatchCreator(employee2);
            specification.MatchAssignee(employee1);
            WorkOrder[] orders = repository.GetWorkOrders(specification);

            Assert.That(orders.Length, Is.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(order1.Id));
        }

        [Test]
        public void ShouldSearchBySpecificationWithStatus()
        {
            new DatabaseTester().Clean();

            var employee1 = new Employee("1", "1", "1", "1");
            var employee2 = new Employee("2", "2", "2", "2");
            var order1 = new WorkOrder();
            order1.Creator = employee2;
            order1.Assignee = employee1;
            order1.Number = "123";
            order1.Status = WorkOrderStatus.Assigned;
            var order2 = new WorkOrder();
            order2.Creator = employee1;
            order2.Assignee = employee2;
            order2.Number = "456";
            order2.Status = WorkOrderStatus.Draft;

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(employee1);
                context.Add(employee2);
                context.Add(order1);
                context.Add(order2);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);
            var specification = new WorkOrderSearchSpecification();
            specification.MatchStatus(WorkOrderStatus.Assigned);
            WorkOrder[] orders = repository.GetWorkOrders(specification);

            Assert.That(orders.Length, Is.EqualTo(1));
            Assert.That(orders[0].Id, Is.EqualTo(order1.Id));
        }

        [Test]
        public void ShouldSaveAuditEntries()
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
            order.AuditEntries.Add(new AuditEntry(creator, DateTime.Now, WorkOrderStatus.Cancelled,
                WorkOrderStatus.Complete));

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(creator);
                context.Add(assignee);
                context.SaveChanges();
            }

            var repository = new WorkOrderRepository(TestHost.NewDbContext());
            repository.Save(order);

            DbContext context2 = TestHost.NewDbContext();
            var rehydratedWorkOrder = context2.Find<WorkOrder>(order.Id);
            var x = (AuditEntry)order.AuditEntries[0];
            var y = (AuditEntry)rehydratedWorkOrder!.AuditEntries[0];
            Assert.That(x.BeginStatus, Is.EqualTo(y.BeginStatus));
        }
    }
}          