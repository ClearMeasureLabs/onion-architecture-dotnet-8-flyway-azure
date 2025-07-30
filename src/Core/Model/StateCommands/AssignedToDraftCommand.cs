using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands
{
    public class AssignedToDraftCommand : StateCommandBase
    {
        public AssignedToDraftCommand(WorkOrder workOrder, Employee currentUser) : base(workOrder, currentUser)
        {
        }

        public override WorkOrderStatus GetBeginStatus()
        {
            return WorkOrderStatus.Assigned;
        }

        protected override WorkOrderStatus GetEndStatus()
        {
            return WorkOrderStatus.Draft;
        }

        protected override bool userCanExecute(Employee currentUser)
        {
            return currentUser == _workOrder.Assignee;
        }

        public override string TransitionVerbPresentTense
        {
            get { return "Reject"; }
        }

        public override string TransitionVerbPastTense
        {
            get { return "Rejected"; }
        }

        protected override void preExecute(IStateCommandVisitor commandVisitor)
        {
        }

        protected override void postExecute(IStateCommandVisitor commandVisitor)
        {
            commandVisitor.EditWorkOrder(_workOrder);
        }
    }
}
