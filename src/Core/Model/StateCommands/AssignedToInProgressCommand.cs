using Core.Model;
using Core.Model.StateCommands;
using Core.Services;

namespace Core.Model.StateCommands
{
	public class AssignedToInProgressCommand : StateCommandBase
	{
	    public AssignedToInProgressCommand(WorkOrder workOrder, Employee currentUser) : base(workOrder, currentUser)
	    {
	    }

	    public override WorkOrderStatus GetBeginStatus()
		{
			return WorkOrderStatus.Assigned;
		}

		protected override WorkOrderStatus GetEndStatus()
		{
			return WorkOrderStatus.InProgress;
		}

		protected override bool userCanExecute(Employee currentUser)
		{
			return currentUser == _workOrder.Assignee;
		}

		public override string TransitionVerbPresentTense
		{
			get { return "Begin"; }
		}

		public override string TransitionVerbPastTense
		{
			get { return "Begun"; }
		}

		protected override void postExecute(IStateCommandVisitor commandVisitor)
		{
			commandVisitor.EditWorkOrder(_workOrder);
		}
	}
}