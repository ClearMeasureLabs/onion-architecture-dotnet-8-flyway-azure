using System;
using System.Diagnostics;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Services.Impl;
using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands
{
	public class DraftToAssignedCommand(WorkOrder workOrder, Employee currentUser)
        : StateCommandBase(workOrder, currentUser)
    {
        public override WorkOrderStatus GetBeginStatus()
		{
			return WorkOrderStatus.Draft;
		}

        public override WorkOrderStatus GetEndStatus()
		{
			return WorkOrderStatus.Assigned;
		}

        protected override bool UserCanExecute(Employee currentUser)
		{
			return currentUser == WorkOrder.Creator;
		}

        public override string TransitionVerbPresentTense => "Assign";

        public override string TransitionVerbPastTense => "Assigned";

        public override void Execute(StateCommandContext context)
        {
            WorkOrder.AssignedDate = context.CurrentDateTime;
            WorkOrder.Assignee = CurrentUser;
            base.Execute(context);
        }
	}
}