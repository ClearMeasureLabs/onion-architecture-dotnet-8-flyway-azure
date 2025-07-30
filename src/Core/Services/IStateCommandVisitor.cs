using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Services
{
	public interface IStateCommandVisitor
	{
		void SaveWorkOrder(WorkOrder workOrder);
		void EditWorkOrder(WorkOrder workOrder);
		void GoToWorkOrderSearch(Employee creator, Employee assignee, WorkOrderStatus status);
		void SendMessage(string message);
		void SendError(string message); 
		T GetService<T>();
	    void GoToDashboard();
	}
}