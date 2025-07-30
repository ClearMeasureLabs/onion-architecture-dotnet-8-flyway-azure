using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Model;
using Shouldly;

namespace UnitTests.Core.Model.StateCommands;

public class AssignedToCancelledCommandTester : StateCommandBaseTester
{
    [Test]
    public void ShouldBeValid()
    {
        var order = new WorkOrder
        {
            Status = WorkOrderStatus.Assigned
        };
        var employee = new Employee();
        order.Creator = employee;

        var command = new AssignedToCancelledCommand(order, employee);
        Assert.That(command.IsValid(), Is.True);
    }

    [Test]
    public void ShouldNotBeValidInWrongStatus()
    {
        var order = new WorkOrder
        {
            Status = WorkOrderStatus.Draft
        };
        var employee = new Employee();
        order.Creator = employee;

        var command = new AssignedToCancelledCommand(order, employee);
        Assert.That(command.IsValid(), Is.False);
    }

    [Test]
    public void ShouldNotBeValidWithWrongEmployee()
    {
        var order = new WorkOrder
        {
            Status = WorkOrderStatus.Assigned
        };
        var employee = new Employee();
        var differentEmployee = new Employee();
        order.Creator = employee;

        var command = new AssignedToCancelledCommand(order, differentEmployee);
        Assert.That(command.IsValid(), Is.False);
    }

    [Test]
    public void ShouldTransitionStateProperly()
    {
        var order = new WorkOrder
        {
            Number = "123",
            Status = WorkOrderStatus.Assigned
        };
        var employee = new Employee();
        order.Creator = employee;

        var command = new AssignedToCancelledCommand(order, employee);
        var visitorStub = new VisitorStub(new StubbedCalendar(DateTime.Now));
        command.Execute(visitorStub, new LoggingNotifier());

        visitorStub.SentMessage.ShouldBe("You have cancelled work order 123");
        visitorStub.SavedWorkOrder.ShouldBe(order);
        visitorStub.EditedWorkOrder.ShouldBe(order);
        Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Cancelled));
    }

    protected override StateCommandBase GetStateCommand(WorkOrder order, Employee employee)
    {
        return new AssignedToCancelledCommand(order, employee);
    }
}

public class VisitorStub(params object[] services) : IStateCommandVisitor
{
    public WorkOrder SavedWorkOrder { get; set; } = null!;
    public WorkOrder EditedWorkOrder { get; set; } = null!;
    public string SentMessage { get; set; } = null!;

    public void SaveWorkOrder(WorkOrder workOrder)
    {
        SavedWorkOrder = workOrder;
    }

    public void EditWorkOrder(WorkOrder workOrder)
    {
        EditedWorkOrder = workOrder;
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
        return (T)services.Single(o => o is T);
    }

    public void GoToDashboard()
    {
        throw new NotImplementedException();
    }
}