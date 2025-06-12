using System;
using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests.Core.Model.StateCommands
{
    public class AssignedToCancelledCommandTester : StateCommandBaseTester
    {

        [Test]
        public void ShouldBeValid()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Assigned;
            var employee = new Employee();
            order.Creator = employee;

            var command = new AssignedToCancelledCommand(order, employee);
            Assert.That(command.IsValid(), Is.True);
        }

        [Test]
        public void ShouldNotBeValidInWrongStatus()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Draft;
            var employee = new Employee();
            order.Creator = employee;

            var command = new AssignedToCancelledCommand(order, employee);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldNotBeValidWithWrongEmployee()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Assigned;
            var employee = new Employee();
            var differentEmployee = new Employee();
            order.Creator = employee;

            var command = new AssignedToCancelledCommand(order, differentEmployee);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldTransitionStateProperly()
        {
            var order = new WorkOrder();
            order.Number = "123";
            order.Status = WorkOrderStatus.Assigned;
            var employee = new Employee();
            order.Creator = employee;

            var mocks = new MockRepository();
            var dynamicMock = mocks.DynamicMock<IStateCommandVisitor>();
            dynamicMock.SaveWorkOrder(order);
            dynamicMock.SendMessage("You have cancelled work order 123");
            dynamicMock.EditWorkOrder(order);
            SetupResult.For(dynamicMock.GetService<ICalendar>()).Return(new StubbedCalendar(DateTime.Now));
            mocks.ReplayAll();

            var command = new AssignedToCancelledCommand(order, employee);
            command.Execute(dynamicMock, new LoggingNotifier());

            mocks.VerifyAll();
            Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Cancelled));
        }

        protected override StateCommandBase GetStateCommand(WorkOrder order, Employee employee)
        {
            return new AssignedToCancelledCommand(order, employee);
        }
    }
}