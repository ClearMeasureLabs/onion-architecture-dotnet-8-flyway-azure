using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Services
{
	public interface IWorkflowFacilitator
	{
		IStateCommand[] GetValidStateCommands(WorkOrder workOrder, Employee currentUser);
	}
}