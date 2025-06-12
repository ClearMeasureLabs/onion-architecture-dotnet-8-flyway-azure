using System;
using Core.Model;
using Core.Model.StateCommands;
using Core.Services;

namespace Core.Model.StateCommands
{
	public class SaveDraftCommand : StateCommandBase
	{
        private ICalendar _calendar;
		
	    public SaveDraftCommand(WorkOrder workOrder, Employee currentUser, ICalendar calendar) : base(workOrder, currentUser)
	    {
	        _calendar = calendar;
	    }

	    public override WorkOrderStatus GetBeginStatus()
		{
			return WorkOrderStatus.Draft;
		}

		protected override WorkOrderStatus GetEndStatus()
		{
			return WorkOrderStatus.Draft;
		}

		protected override bool userCanExecute(Employee currentUser)
		{
			return currentUser == _workOrder.Creator;
		}

		public override string TransitionVerbPresentTense
		{
			get { return "Save"; }
		}

		public override string TransitionVerbPastTense
		{
			get { return "Saved"; }
		}
        protected override void preExecute(IStateCommandVisitor commandVisitor)
        {
           if (_workOrder.CreatedDate.Equals(null))
           {
               _workOrder.CreatedDate = DateTime.Now;
           }
        }
		protected override void postExecute(IStateCommandVisitor commandVisitor)
		{
		   // _workOrder.CreatedDate = _calendar.GetCurrentTime();
			commandVisitor.EditWorkOrder(_workOrder);
		}

        protected override void sendChangeStateNotification(INotifier notifier)
        { 
            //do nothing because state didn't change
        }
	}
}