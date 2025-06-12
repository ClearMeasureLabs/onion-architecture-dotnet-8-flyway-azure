using System;
using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using Core.Services.Impl;

namespace Core.Model.StateCommands
{
	public class DraftToAssignedCommand : StateCommandBase
	{
	    private ICalendar _calendar;

	    public DraftToAssignedCommand(WorkOrder workOrder, Employee currentUser, ICalendar calendar) : base(workOrder, currentUser)
	    {
	        _calendar = calendar;
	    }
        
	    public override WorkOrderStatus GetBeginStatus()
		{
			return WorkOrderStatus.Draft;
		}

		protected override WorkOrderStatus GetEndStatus()
		{
			return WorkOrderStatus.Assigned;
		}

		protected override bool userCanExecute(Employee currentUser)
		{
			return currentUser == _workOrder.Creator;
		}

		public override string TransitionVerbPresentTense
		{
			get { return "Assign"; }
		}

		public override string TransitionVerbPastTense
		{
			get { return "Assigned"; }
		}

	    protected override void preExecute(IStateCommandVisitor commandVisitor)
        {
            _workOrder.AssignedDate = DateTime.Now;
        }

	    protected override void postExecute(IStateCommandVisitor commandVisitor)
		{
			commandVisitor.EditWorkOrder(_workOrder);
		}

        protected override void sendAssignedNotification(INotifier notifier)
        {
            notifier.SendAssignedNotification(string.Format("Work order {0} assigned to you.", _workOrder.Number), _workOrder.Assignee);
        }
	}
}