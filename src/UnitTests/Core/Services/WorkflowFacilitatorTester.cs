using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using Core.Services.Impl;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace UnitTests.Core.Services;

[TestFixture]
public class WorkflowFacilitatorTester
{
    [Test]
    public void ShouldGetNoValidStateCommandsForWrongUser()
    {
        var facilitator = new WorkflowFacilitator(new StubbedCalendar(new DateTime(2000, 1, 1)));
        var workOrder = new WorkOrder();
        var employee = new Employee();
        var commands = facilitator.GetValidStateCommands(workOrder, employee);

        Assert.That(commands.Length, Is.EqualTo(0));
    }

    [Test]
    public void ShouldReturnAllStateCommandsInCorrectOrder()
    {
        var facilitator = new WorkflowFacilitator(new StubbedCalendar(new DateTime(2000, 1, 1)));
        var commands = facilitator.GetAllStateCommands(new WorkOrder(), new Employee());

        Assert.That(commands.Length, Is.EqualTo(10));

        Assert.That(commands[0], Is.InstanceOf(typeof(SaveDraftCommand)));
        Assert.That(commands[1], Is.InstanceOf(typeof(DraftToAssignedCommand)));
        Assert.That(commands[2], Is.InstanceOf(typeof(AssignedToDraftCommand)));
        Assert.That(commands[3], Is.InstanceOf(typeof(AssignedToInProgressCommand)));
        Assert.That(commands[4], Is.InstanceOf(typeof(InProgressToAssignedCommand)));
        Assert.That(commands[5], Is.InstanceOf(typeof(InProgressToCompleteCommand)));
        Assert.That(commands[6], Is.InstanceOf(typeof(CompleteToAssignedCommand)));
        Assert.That(commands[7], Is.InstanceOf(typeof(AssignedToDraftForWithdrawCommand)));
        Assert.That(commands[8], Is.InstanceOf(typeof(InProgressToCancelledCommand)));
        Assert.That(commands[9], Is.InstanceOf(typeof(AssignedToCancelledCommand)));
    }

    [Test]
    public void ShouldFilterFullListToReturnValidCommands()
    {
        var stubFacilitator = new StubWorkflowFacilitator(new StubbedCalendar(new DateTime(2000, 1, 1)));
        var commandsToReturn = new IStateCommand[]
        {
            new StubbedStateCommand(true),
            new StubbedStateCommand(true),
            new StubbedStateCommand(false)
        };

        stubFacilitator.CommandsToReturn = commandsToReturn;

        var commands = stubFacilitator.GetValidStateCommands(null!, null!);

        Assert.That(commands.Length, Is.EqualTo(2));
    }

    public class StubWorkflowFacilitator(ICalendar calendar) : WorkflowFacilitator(calendar)
    {
        public IStateCommand[] CommandsToReturn { get; set; } = null!;

        public override IStateCommand[] GetAllStateCommands(WorkOrder workOrder, Employee employee)
        {
            return CommandsToReturn;
        }
    }

    public class StubbedStateCommand : IStateCommand
    {
        private readonly bool _isValid;

        public StubbedStateCommand(bool isValid)
        {
            _isValid = isValid;
        }

        public bool IsValid()
        {
            return _isValid;
        }

        public void Execute(IStateCommandVisitor commandVisitor, INotifier notifier)
        {
            throw new NotImplementedException();
        }

        public string TransitionVerbPresentTense => throw new NotImplementedException();

        public bool Matches(string commandName)
        {
            throw new NotImplementedException();
        }

        public WorkOrderStatus GetBeginStatus()
        {
            throw new NotImplementedException();
        }
    }
}