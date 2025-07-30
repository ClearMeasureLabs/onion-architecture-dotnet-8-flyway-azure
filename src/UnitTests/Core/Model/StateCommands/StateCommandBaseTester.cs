using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace UnitTests.Core.Model.StateCommands;

public abstract class StateCommandBaseTester
{
    protected abstract StateCommandBase GetStateCommand(WorkOrder order, Employee employee);


    [Test]
    public virtual void SendChangeStateNotificationShouldSendWhenStatusChanges()
    {
        var order = new WorkOrder
        {
            Status = WorkOrderStatus.Complete
        };
        var employee = new Employee();
        order.Creator = employee;

        var commandVisitor = new VisitorStub();

        var stateCommandBase = GetStateCommand(order, employee);

        var notifierMock = new NotifierMock();
        stateCommandBase.Execute(commandVisitor, notifierMock);

        if (stateCommandBase.TransitionVerbPastTense == "Saved")
            Assert.That(notifierMock.SentMessage1 == null);
        else
            Assert.That(notifierMock.SentMessage1 != null);
    }

    [Test]
    public void ShouldSendMessageWhenStateChangedToAssigned()
    {
        var order = new WorkOrder();
        order.Number = "123";
        order.Status = WorkOrderStatus.Assigned;
        var employee = new Employee();
        order.Assignee = employee;

        var commandVisitor = new VisitorStub();

        var command = GetStateCommand(order, employee);
        var notifierMock = new NotifierMock();
        command.Execute(commandVisitor, notifierMock);

        if (command.ShouldSendAssignmentNotification())
            Assert.That(notifierMock.SentMessage != null);
        else
            Assert.That(notifierMock.SentMessage == null);
    }

    public class NotifierMock : INotifier
    {
        public Employee? SentEmployee;
        public string? SentMessage;
        public string? SentMessage1;

        public void SendAssignedNotification(string message, Employee employee)
        {
            SentMessage = message;
            SentEmployee = employee;
        }

        public void SendChangeStateNotification(string message)
        {
            SentMessage1 = message;
        }
    }
}