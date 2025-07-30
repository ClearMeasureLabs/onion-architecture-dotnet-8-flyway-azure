using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands
{
    public class InProgressToCancelledCommand : StateCommandBase
    {
         
        public InProgressToCancelledCommand(WorkOrder workOrder, Employee currentUser) : base(workOrder, currentUser)
        {
        }

        public override WorkOrderStatus GetBeginStatus()
        {
            return WorkOrderStatus.InProgress;
        }

        protected override WorkOrderStatus GetEndStatus()
        {
            return WorkOrderStatus.Cancelled;
        }

        protected override bool userCanExecute(Employee currentUser)
        {
            return currentUser == _workOrder.Creator;
        }

        public override string TransitionVerbPresentTense
        {
            get { return "Cancel"; }
        }

        public override string TransitionVerbPastTense
        {
            get { return "Cancelled"; }
        }

        protected override void postExecute(IStateCommandVisitor commandVisitor)
        {
            commandVisitor.EditWorkOrder(_workOrder);
        }
    }
}