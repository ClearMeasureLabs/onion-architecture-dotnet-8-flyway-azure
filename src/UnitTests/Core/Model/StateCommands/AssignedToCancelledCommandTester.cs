using System;
using System.Collections.ObjectModel;
using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;

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

            var command = new AssignedToCancelledCommand(order, employee);
            VisitorStub visitorStub = new VisitorStub(new StubbedCalendar(DateTime.Now));
            command.Execute(visitorStub, new LoggingNotifier());

            visitorStub.SentMessage.ShouldBe("You have cancelled work order 123");
            visitorStub.SavedWorkOrder.ShouldBe(order);
            visitorStub.EdittedWorkOrder.ShouldBe(order);
            Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Cancelled));
        }

        protected override StateCommandBase GetStateCommand(WorkOrder order, Employee employee)
        {
            return new AssignedToCancelledCommand(order, employee);
        }
    }

    public class VisitorStub : IStateCommandVisitor
    {
        private object[] _services = [];
        public WorkOrder SavedWorkOrder { get; set; }
        public WorkOrder EdittedWorkOrder { get; set; }
        public string SentMessage { get; set; }

        public VisitorStub(params object[] services)
        {
            _services = services;
        }
        public void SaveWorkOrder(WorkOrder workOrder)
        {
            SavedWorkOrder = workOrder;
        }

        public void EditWorkOrder(WorkOrder workOrder)
        {
            EdittedWorkOrder = workOrder;
        }

        public void GoToWorkOrderSearch(Employee creator, Employee assignee, WorkOrderStatus status)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string message)
        {
            SentMessage = message;
        }

        public void SendError(string message)
        {
            throw new NotImplementedException();
        }

        public T GetService<T>()
        {
            return (T)_services.Single(o => o is T);
        }

        public void GoToDashboard()
        {
            throw new NotImplementedException();
        }
    }
}