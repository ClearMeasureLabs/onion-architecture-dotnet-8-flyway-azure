using Core.Model;
using System.Threading.Tasks;

namespace Core.Services
{
	public interface IWorkOrderRepository
	{
		Task SaveAsync(WorkOrder workOrder);
		Task<WorkOrder?> GetWorkOrderAsync(string number);
		// Task<WorkOrder[]> GetWorkOrdersAsync(WorkOrderSearchSpecification specification);
	}
}