using System.Collections.Generic;
using ClearMeasure.Bootcamp.Core.Model.StateCommands;
using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Services.Impl
{
	public class WorkflowFacilitator : IWorkflowFacilitator
	{
	    public IStateCommand[] GetValidStateCommands(WorkOrder workOrder, Employee currentUser)
		{
			List<IStateCommand> commands = new List<IStateCommand>(
                GetAllStateCommands(workOrder, currentUser));
			commands.RemoveAll(delegate(IStateCommand obj) { return !obj.IsValid(); });

			return commands.ToArray();
		}

		public virtual IStateCommand[] GetAllStateCommands(WorkOrder workOrder, Employee currentUser)
		{
			List<IStateCommand> commands = new List<IStateCommand>();
            commands.Add(new SaveDraftCommand(workOrder, currentUser));
            commands.Add(new DraftToAssignedCommand(workOrder, currentUser));
            commands.Add(new AssignedToInProgressCommand(workOrder, currentUser));
			commands.Add(new InProgressToCompleteCommand(workOrder, currentUser));

			return commands.ToArray();	
		}
	}
}
