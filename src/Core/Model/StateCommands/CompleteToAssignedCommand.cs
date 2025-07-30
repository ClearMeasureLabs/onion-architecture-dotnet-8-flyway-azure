using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands 
{
   public  class CompleteToAssignedCommand : StateCommandBase
    {

       public CompleteToAssignedCommand(WorkOrder workOrder, Employee currentUser): base(workOrder, currentUser)
        {
        }

        public override WorkOrderStatus GetBeginStatus()
        {
            return WorkOrderStatus.Complete;
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
            get { return "Reassign"; }
        }

        public override string TransitionVerbPastTense
        {
            get { return "Reassigned"; }
        }

        protected override void preExecute(IStateCommandVisitor commandVisitor)
        {
        }

        protected override void postExecute(IStateCommandVisitor commandVisitor)
        {
            commandVisitor.EditWorkOrder(_workOrder);
        }

        protected override void sendAssignedNotification(INotifier notifier)
        {
            notifier.SendAssignedNotification(string.Format("Work order {0} assigned to you.", _workOrder.Number), 
                _workOrder.Assignee ?? throw new InvalidOperationException());
        }
    }
}
