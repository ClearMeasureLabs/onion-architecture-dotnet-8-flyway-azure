using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Handlers;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;
using Shouldly;

namespace ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.DataAccess
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
            WorkOrder order123 = (await repository.GetWorkOrderAsync("123"))!;
            WorkOrder order456 = (await repository.GetWorkOrderAsync("456"))!;

            order123.Id.ShouldBe(order1.Id);
            order456.Id.ShouldBe(order2.Id);
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
                rehydratedWorkOrder.Id.ShouldBe(order.Id);
                rehydratedWorkOrder.Creator!.Id.ShouldBe(order.Creator.Id);
                rehydratedWorkOrder.Assignee!.Id.ShouldBe(order.Assignee.Id);
                rehydratedWorkOrder.Title.ShouldBe(order.Title);
                rehydratedWorkOrder.Description.ShouldBe(order.Description);
                rehydratedWorkOrder.Status.ShouldBe(order.Status);
                rehydratedWorkOrder.RoomNumber.ShouldBe(order.RoomNumber);
                rehydratedWorkOrder.Number.ShouldBe(order.Number);
            }
        }

        [Test]
        public async Task ShouldSearchBySpecificationWithAssignee()
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
            WorkOrder[] orders = await repository.GetWorkOrdersAsync(specification);

            orders.Length.ShouldBe(1);
            orders[0].Id.ShouldBe(order1.Id);
        }

        [Test]
        public async Task ShouldSearchBySpecificationWithCreator()
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
            WorkOrder[] orders = await repository.GetWorkOrdersAsync(specification);

            orders.Length.ShouldBe(1);
            orders[0].Id.ShouldBe(order1.Id);
        }

        [Test]
        public async Task ShouldSearchBySpecificationWithFullSpecification()
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
            WorkOrder[] orders = await repository.GetWorkOrdersAsync(specification);

            orders.Length.ShouldBe(1);
            orders[0].Id.ShouldBe(order1.Id);
        }

        [Test]
        public async Task ShouldSearchBySpecificationWithStatus()
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
            WorkOrder[] orders = await repository.GetWorkOrdersAsync(specification);

            orders.Length.ShouldBe(1);
            orders[0].Id.ShouldBe(order1.Id);
        }

        [Test]
        public async Task ShouldSearchWithEmptySpecificationAndReturnAll()
        {
            new DatabaseTester().Clean();

            var employee = new Employee("1", "1", "1", "1");
            var order1 = new WorkOrder { Creator = employee, Assignee = employee, Number = "123" };
            var order2 = new WorkOrder { Creator = employee, Assignee = employee, Number = "456" };

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(order1);
                context.Add(order2);
                await context.SaveChangesAsync();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);
            WorkOrder[] orders = await repository.GetWorkOrdersAsync(new WorkOrderSearchSpecification());

            orders.Length.ShouldBe(2);
            orders.Any(o => o.Id == order1.Id).ShouldBeTrue();
            orders.Any(o => o.Id == order2.Id).ShouldBeTrue();
        }
        
        

        [Test]
        public async Task ShouldSaveAuditEntries()
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

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);
            await repository.SaveAsync(order);

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                var rehydratedWorkOrder = context.Set<WorkOrder>()
                    .Include(wo => wo.AuditEntries)
                    .Single(wo => wo.Id == order.Id);
                var x = (AuditEntry)order.AuditEntries[0];
                var y = (AuditEntry)rehydratedWorkOrder.AuditEntries[0];
                x.BeginStatus.ShouldBe(y.BeginStatus);
            }
        }

        [Test]
        public void SearchShouldReturnHydratedEmployeesWithWorkOrders()
        {
            new DatabaseTester().Clean();

            var creator = new Employee("1", "John", "Doe", "john.doe@example.com");
            var assignee = new Employee("2", "Jane", "Smith", "jane.smith@example.com");

            var order1 = new WorkOrder
            {
                Creator = creator,
                Assignee = assignee,
                Number = "123",
                Title = "Fix plumbing",
                Description = "Fix the plumbing in room 101",
                RoomNumber = "101",
                Status = WorkOrderStatus.InProgress
            };

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(creator);
                context.Add(assignee);
                context.Add(order1);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            var repository = new WorkOrderRepository(dataContext);

            var specification = new WorkOrderSearchSpecification();
            specification.MatchCreator(creator);

            var orders = repository.GetWorkOrdersAsync(specification).Result;

            orders.Length.ShouldBe(1);

            var rehydratedOrder = orders.First(o => o.Number == "123");

            rehydratedOrder.Creator.ShouldNotBeNull();
            rehydratedOrder.Assignee.ShouldNotBeNull();
            rehydratedOrder.Creator.Id.ShouldBe(creator.Id);
            rehydratedOrder.Creator.FirstName.ShouldBe(creator.FirstName);
            rehydratedOrder.Creator.LastName.ShouldBe(creator.LastName);
            rehydratedOrder.Creator.EmailAddress.ShouldBe(creator.EmailAddress);
            rehydratedOrder.Assignee.Id.ShouldBe(assignee.Id);
            rehydratedOrder.Assignee.FirstName.ShouldBe(assignee.FirstName);
            rehydratedOrder.Assignee.LastName.ShouldBe(assignee.LastName);
            rehydratedOrder.Assignee.EmailAddress.ShouldBe(assignee.EmailAddress);

        }
    }
}