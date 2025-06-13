using Core.Model;

namespace Core.Services
{
	public interface IWorkOrderRepository
	{
		void Save(WorkOrder workOrder);
		WorkOrder? GetWorkOrder(string number);
		WorkOrder[] GetWorkOrders(WorkOrderSearchSpecification specification);
	}
}