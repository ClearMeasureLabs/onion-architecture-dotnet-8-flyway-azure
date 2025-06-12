using Core.Model;

namespace Core.Services
{
	public interface IWorkflowFacilitator
	{
		IStateCommand[] GetValidStateCommands(WorkOrder workOrder, Employee currentUser);
	}
}