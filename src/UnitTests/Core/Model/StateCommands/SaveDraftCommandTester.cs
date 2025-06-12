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
    public class SaveDraftCommandTester : StateCommandBaseTester
    {
        private ICalendar _calendar = new StubbedCalendar(new DateTime(2008, 3, 14));
        [Test]
        public void ShouldNotBeValidInWrongStatus()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Complete;
            var employee = new Employee();
            order.Creator = employee;

            var command = new SaveDraftCommand(order, employee,_calendar);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldNotBeValidWithWrongEmployee()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Draft;
            var employee = new Employee();
            order.Creator = employee;

            var command = new SaveDraftCommand(order, new Employee(),_calendar);
            Assert.That(command.IsValid(), Is.False);
        }

        [Test]
        public void ShouldBeValid()
        {
            var order = new WorkOrder();
            order.Status = WorkOrderStatus.Draft;
            var employee = new Employee();
            order.Creator = employee;

            var command = new SaveDraftCommand(order, employee,_calendar);
            Assert.That(command.IsValid(), Is.True);
        }

        [Test]
        public void ShouldTransitionStateProperly()
        {
            var order = new WorkOrder();
            order.Number = "123";
            order.Status = WorkOrderStatus.Draft;
            var employee = new Employee();
            order.Creator = employee;

            var mocks = new MockRepository();
            var commandVisitor = mocks.StrictMock<IStateCommandVisitor>();
            commandVisitor.SaveWorkOrder(order);
            commandVisitor.SendMessage("You have saved work order 123");
            commandVisitor.EditWorkOrder(order);
            SetupResult.For(commandVisitor.GetService<ICalendar>()).Return(new StubbedCalendar(DateTime.Now));
            mocks.ReplayAll();

            var command = new SaveDraftCommand(order, employee,_calendar);
            command.Execute(commandVisitor, new LoggingNotifier());

            mocks.VerifyAll();
            Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Draft));
            Assert.That(order.CreatedDate, Is.Not.Null);
        }

        protected override StateCommandBase GetStateCommand(WorkOrder order, Employee employee)
        {
            return new SaveDraftCommand(order, employee,_calendar);
        }
    }
}