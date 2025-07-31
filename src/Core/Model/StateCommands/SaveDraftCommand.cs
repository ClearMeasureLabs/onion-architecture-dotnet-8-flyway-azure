using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands;

public class SaveDraftCommand(WorkOrder workOrder, Employee currentUser) : StateCommandBase(workOrder,
    currentUser)
{
    public override WorkOrderStatus GetBeginStatus()
    {
        return WorkOrderStatus.Draft;
    }

    public override WorkOrderStatus GetEndStatus()
    {
        return WorkOrderStatus.Draft;
    }

    protected override bool UserCanExecute(Employee currentUser)
    {
        return currentUser == WorkOrder.Creator;
    }

    public override string TransitionVerbPresentTense => "Save";

    public override string TransitionVerbPastTense => "Saved";

    public override void Execute(StateCommandContext context)
    {
        if (WorkOrder.CreatedDate.Equals(null)) WorkOrder.CreatedDate = context.CurrentDateTime;
    }
}