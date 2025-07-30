using System;
using Core.Model;
using Core.Model.StateCommands;
using Core.Services;
using Core.Services.Impl;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace Core.Model.StateCommands
{
	public class InProgressToCompleteCommand : StateCommandBase
	{
        private ICalendar _calendar;

	    public InProgressToCompleteCommand(WorkOrder workOrder, Employee currentUser, ICalendar calendar) : base(workOrder, currentUser)
	    {
	        _calendar = calendar;
	    }

	    public override string TransitionVerbPresentTense
		{
			get { return "Complete"; }
		}

		public override string TransitionVerbPastTense
		{
			get { return "Completed"; }
		}

		public override WorkOrderStatus GetBeginStatus()
		{
			return WorkOrderStatus.InProgress;
		}

		protected override WorkOrderStatus GetEndStatus()
		{
			return WorkOrderStatus.Complete;
		}

		protected override bool userCanExecute(Employee currentUser)
		{
			return currentUser == _workOrder.Assignee;
		}

        protected override void preExecute(IStateCommandVisitor commandVisitor)
        {
            _calendar.GetCurrentTime();
            _workOrder.CompletedDate = DateTime.Now;
        }
		
        protected override void postExecute(IStateCommandVisitor commandVisitor)
		{
			commandVisitor.EditWorkOrder(_workOrder);
		}
	}
}