using System;
using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests.Core.Model.StateCommands
{
    [TestFixture]
    public class AssignedToInProgressCommandTester : StateCommandBaseTester
    {
        [Test]
        public void ShouldNotBeValidInWrongStatus()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Draft;
            var employee = new Employee();
            order.Assignee = employee;

            var command = new AssignedToInProgressCommand(order, employee);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldNotBeValidWithWrongEmployee()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Assigned;
            var employee = new Employee();
            order.Assignee = employee;

            var command = new AssignedToInProgressCommand(order, new Employee());
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldBeValid()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Assigned;
            var employee = new Employee();
            order.Assignee = employee;

            var command = new AssignedToInProgressCommand(order, employee);
            Assert.That(command.IsValid(), Is.True);
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
            var commandVisitor = mocks.StrictMock<IStateCommandVisitor>();
            commandVisitor.SaveWorkOrder(order);
            commandVisitor.SendMessage("You have begun work order 123");
            commandVisitor.EditWorkOrder(order);
            SetupResult.For(commandVisitor.GetService<ICalendar>()).Return(new StubbedCalendar(DateTime.Now));
            mocks.ReplayAll();

            var command = new AssignedToInProgressCommand(order, employee);
            command.Execute(commandVisitor, new LoggingNotifier());

            mocks.VerifyAll();
            Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.InProgress));
        }

        protected override StateCommandBase GetStateCommand(WorkOrder order, Employee employee)
        {
            return new AssignedToInProgressCommand(order, employee);
        }
    }
}