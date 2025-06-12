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
    public class InProgressToCompleteCommandTester : StateCommandBaseTester
    {
        private ICalendar _calendar = new StubbedCalendar(new DateTime(2008, 3, 14));
        [Test]
        public void ShouldNotBeValidInWrongStatus()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Complete;
            var employee = new Employee();
            order.Assignee = employee;
           
            var command = new InProgressToCompleteCommand(order, employee,_calendar);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldNotBeValidWithWrongEmployee()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.InProgress;
            var employee = new Employee();
            order.Assignee = employee;

            var command = new InProgressToCompleteCommand(order, new Employee(),_calendar);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldBeValid()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.InProgress;
            var employee = new Employee();
            order.Assignee = employee;

            var command = new InProgressToCompleteCommand(order, employee,_calendar);
            Assert.That(command.IsValid(), Is.True);
        }

        [Test]
        public void ShouldTransitionStateProperly()
        {
            var order = new WorkOrder();
            order.Number = "123";
            order.Status = WorkOrderStatus.InProgress;
            var employee = new Employee();
            order.Assignee = employee;

            var mocks = new MockRepository();
            var commandVisitor = mocks.StrictMock<IStateCommandVisitor>();

            commandVisitor.SaveWorkOrder(order);
            commandVisitor.SendMessage("You have completed work order 123");
            commandVisitor.EditWorkOrder(order);
            SetupResult.For(commandVisitor.GetService<ICalendar>()).Return(new StubbedCalendar(DateTime.Now));

            var command = new InProgressToCompleteCommand(order, employee,_calendar);

            mocks.ReplayAll();


            command.Execute(commandVisitor, new LoggingNotifier());

            mocks.VerifyAll();
            Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Complete));
            Assert.That(order.CompletedDate, Is.Not.Null);
        }

        protected override StateCommandBase GetStateCommand(WorkOrder order, Employee employee)
        {
            return new InProgressToCompleteCommand(order, employee,_calendar);
        }
    }
}