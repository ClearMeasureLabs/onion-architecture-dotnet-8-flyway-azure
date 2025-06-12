using System;
using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests.Core.Model.StateCommands
{
    public class AssignedToDraftCommandTester
    {
        [Test]
        public void ShouldBeValid()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Assigned;
            var employee = new Employee();
            order.Assignee = employee;

            var command = new AssignedToDraftCommand(order, employee);
            Assert.That(command.IsValid(), Is.True);
        }

        [Test]
        public void ShouldNotBeValidInWrongStatus()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Draft;
            var employee = new Employee();
            order.Assignee = employee;

            var command = new AssignedToDraftCommand(order, employee);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldTransitionStateProperly()
        {
            var order = new WorkOrder();
            order.Number = "123";
            order.Status = WorkOrderStatus.Assigned;
            var employee = new Employee();
            order.Assignee = employee;

            var mocks = new MockRepository();
            var commandVisitor = mocks.DynamicMock<IStateCommandVisitor>();
            commandVisitor.SaveWorkOrder(order);
            commandVisitor.SendMessage("You have rejected work order 123");
            commandVisitor.EditWorkOrder(order);
            SetupResult.For(commandVisitor.GetService<ICalendar>()).Return(new StubbedCalendar(DateTime.Now));
            mocks.ReplayAll();

            var command = new AssignedToDraftCommand(order, employee);
            command.Execute(commandVisitor, new LoggingNotifier());

            mocks.VerifyAll();
            Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Draft));
        }

        [Test]
        public void ShouldNotBeValidWithWrongEmployee()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Assigned;
            var employee = new Employee();
            var differentEmployee = new Employee();
            order.Creator = employee;

            var command = new AssignedToDraftCommand(order, differentEmployee);
            Assert.That(command.IsValid(), Is.False);
        }
    }
}
