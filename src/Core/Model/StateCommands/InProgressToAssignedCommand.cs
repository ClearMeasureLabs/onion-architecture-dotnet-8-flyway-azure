using System.Diagnostics;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands
{
    public class InProgressToAssignedCommand : StateCommandBase
    {
        
        public InProgressToAssignedCommand(WorkOrder workOrder, Employee currentUser) : base(workOrder, currentUser)
        {
        }

        public override WorkOrderStatus GetBeginStatus()
        {
            return WorkOrderStatus.InProgress;
        }

        protected override WorkOrderStatus GetEndStatus()
        {
            return WorkOrderStatus.Assigned;
        }

        protected override bool userCanExecute(Employee currentUser)
        {
            return currentUser == _workOrder.Assignee;
        }

        public override string TransitionVerbPresentTense
        {
            get { return "Shelve"; }
        }

        public override string TransitionVerbPastTense
        {
            get { return "Shelved"; }
        }

        protected override void postExecute(IStateCommandVisitor commandVisitor)
        {
            commandVisitor.EditWorkOrder(_workOrder);
        }

        protected override void sendAssignedNotification(INotifier notifier)
        {
            Debug.Assert(_workOrder.Assignee != null, "_workOrder.Assignee != null");
            notifier.SendAssignedNotification(string.Format("Work order {0} assigned to you.", _workOrder.Number), _workOrder.Assignee);
        }
    }
}