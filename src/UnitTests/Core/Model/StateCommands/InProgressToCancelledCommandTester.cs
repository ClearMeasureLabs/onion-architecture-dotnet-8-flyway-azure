using System;
using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using NUnit.Framework;
using Shouldly;

namespace UnitTests.Core.Model.StateCommands
{
    public class InProgressToCancelledCommandTester
    {
        [Test]
        public void ShouldBeValid()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.InProgress;
            var employee = new Employee();
            order.Creator = employee;

            var command = new InProgressToCancelledCommand(order, employee);
            Assert.That(command.IsValid(), Is.True);
        }

        [Test]
        public void ShouldNotBeValidInWrongStatus()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Draft;
            var employee = new Employee();
            order.Creator = employee;

            var command = new InProgressToCancelledCommand(order, employee);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldTransitionStateProperly()
        {
            var order = new WorkOrder();
            order.Number = "123";
            order.Status = WorkOrderStatus.InProgress;
            var employee = new Employee();
            order.Creator = employee;

            var visitorStub = new VisitorStub(new StubbedCalendar(DateTime.Now));
            
            var command = new InProgressToCancelledCommand(order, employee);
            command.Execute(visitorStub, new LoggingNotifier());

            visitorStub.SentMessage.ShouldBe("You have cancelled work order 123");
            visitorStub.SavedWorkOrder.ShouldBe(order);
            visitorStub.EditedWorkOrder.ShouldBe(order);
            Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Cancelled));
        }

        [Test]
        public void ShouldNotBeValidWithWrongEmployee()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.InProgress;
            var employee = new Employee();
            var differentEmployee = new Employee();
            order.Assignee = employee;

            var command = new InProgressToCancelledCommand(order, differentEmployee);
            Assert.That(command.IsValid(), Is.False);
        }
    }
}