using Core.Model;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace Core.Services
{
	public interface IWorkflowFacilitator
	{
		IStateCommand[] GetValidStateCommands(WorkOrder workOrder, Employee currentUser);
	}
}