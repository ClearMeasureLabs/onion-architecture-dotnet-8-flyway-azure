using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands;

public class InProgressToCompleteCommand(WorkOrder workOrder, Employee currentUser) : StateCommandBase(workOrder,
    currentUser)
{
    public override string TransitionVerbPresentTense => "Complete";

    public override string TransitionVerbPastTense => "Completed";

    public override WorkOrderStatus GetBeginStatus()
    {
        return WorkOrderStatus.InProgress;
    }

    public override WorkOrderStatus GetEndStatus()
    {
        return WorkOrderStatus.Complete;
    }

    protected override bool UserCanExecute(Employee currentUser)
    {
        return currentUser == WorkOrder.Assignee;
    }

    public override void Execute(StateCommandContext context)
    {
        WorkOrder.CompletedDate = context.CurrentDateTime;
        base.Execute(context);
    }
}