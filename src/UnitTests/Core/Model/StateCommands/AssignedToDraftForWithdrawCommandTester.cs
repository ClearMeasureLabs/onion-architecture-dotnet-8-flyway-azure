using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Model;
using Shouldly;
using ClearMeasure.Bootcamp.UnitTests.Core;

namespace ClearMeasure.Bootcamp.UnitTests.Core.Model.StateCommands;

internal class AssignedToDraftForWithdrawCommandTester
{
    [Test]
    public void ShouldBeValid()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Assigned;
        var employee = new Employee();
        order.Creator = employee;

        var command = new AssignedToDraftForWithdrawCommand(order, employee);
        Assert.That(command.IsValid(), Is.True);
    }

    [Test]
    public void ShouldNotBeValidInWrongStatus()
    {
        var order = new WorkOrder();
        order.Status = WorkOrderStatus.Draft;
        var employee = new Employee();
        order.Creator = employee;

        var command = new AssignedToDraftForWithdrawCommand(order, employee);
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

        var visitorStub = new VisitorStub(new StubbedCalendar(DateTime.Now));

        var command = new AssignedToDraftForWithdrawCommand(order, employee);
        command.Execute(visitorStub, new LoggingNotifier());

        visitorStub.SentMessage.ShouldBe("You have withdrawn work order 123");
        visitorStub.SavedWorkOrder.ShouldBe(order);
        visitorStub.EditedWorkOrder.ShouldBe(order);
        Assert.That(order.Status, Is.EqualTo(WorkOrderStatus.Draft));
    }
}