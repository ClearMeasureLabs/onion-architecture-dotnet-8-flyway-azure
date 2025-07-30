using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Model;
using Shouldly;

namespace ClearMeasure.Bootcamp.UnitTests.Core.Model.StateCommands;

internal class CompleteToAssignedCommandTester
{
    [Test]
    public void ShouldBeValid()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Complete;
        var employee = new Employee();
        order.Assignee = employee;

        var command = new CompleteToAssignedCommand(order, employee);
        Assert.That(command.IsValid(), Is.True);
    }

    [Test]
    public void ShouldNotBeValidInWrongStatus()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Draft;
        var employee = new Employee();
        order.Assignee = employee;

        var command = new CompleteToAssignedCommand(order, employee);
        Assert.That(command.IsValid(), Is.False);
    }

    [Test]
    public void ShouldTransitionStateProperly()
    {
        var order = new WorkOrder();
        order.Number = "123";
        order.Status = WorkOrderStatus.Complete;
        var employee = new Employee();
        order.Assignee = employee;

        var visitorStub = new VisitorStub();

        var command = new CompleteToAssignedCommand(order, employee);
        command.Execute(visitorStub, new LoggingNotifier());

        visitorStub.SentMessage.ShouldBe("You have reassigned work order 123");
        visitorStub.SavedWorkOrder.ShouldBe(order);
        visitorStub.EditedWorkOrder.ShouldBe(order);
        Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Assigned));
    }

    [Test]
    public void ShouldNotBeValidWithWrongEmployee()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Complete;
        var employee = new Employee();
        var differentEmployee = new Employee();
        order.Creator = employee;

        var command = new CompleteToAssignedCommand(order, differentEmployee);
        Assert.That(command.IsValid(), Is.False);
    }
}