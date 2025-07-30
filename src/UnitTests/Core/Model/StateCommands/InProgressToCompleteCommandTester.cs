using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Model;
using Shouldly;
using ClearMeasure.Bootcamp.UnitTests.Core;

namespace ClearMeasure.Bootcamp.UnitTests.Core.Model.StateCommands;

[TestFixture]
public class InProgressToCompleteCommandTester : StateCommandBaseTester
{
    private readonly ICalendar _calendar = new StubbedCalendar(new DateTime(2008, 3, 14));

    [Test]
    public void ShouldNotBeValidInWrongStatus()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Complete;
        var employee = new Employee();
        order.Assignee = employee;

        var command = new InProgressToCompleteCommand(order, employee, _calendar);
        Assert.That(command.IsValid(), Is.False);
    }

    [Test]
    public void ShouldNotBeValidWithWrongEmployee()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.InProgress;
        var employee = new Employee();
        order.Assignee = employee;

        var command = new InProgressToCompleteCommand(order, new Employee(), _calendar);
        Assert.That(command.IsValid(), Is.False);
    }

    [Test]
    public void ShouldBeValid()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.InProgress;
        var employee = new Employee();
        order.Assignee = employee;

        var command = new InProgressToCompleteCommand(order, employee, _calendar);
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

        var visitorStub = new VisitorStub(new StubbedCalendar(DateTime.Now));

        var command = new InProgressToCompleteCommand(order, employee, _calendar);
        command.Execute(visitorStub, new LoggingNotifier());

        visitorStub.SentMessage.ShouldBe("You have completed work order 123");
        visitorStub.SavedWorkOrder.ShouldBe(order);
        visitorStub.EditedWorkOrder.ShouldBe(order);
        Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Complete));
        Assert.That(order.CompletedDate, Is.Not.Null);
    }

    protected override StateCommandBase GetStateCommand(WorkOrder order, Employee employee)
    {
        return new InProgressToCompleteCommand(order, employee, _calendar);
    }
}