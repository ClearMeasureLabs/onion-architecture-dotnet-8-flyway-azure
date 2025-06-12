using Core.Model;

namespace Core.Services
{
	public interface IWorkOrderBuilder
	{
		WorkOrder CreateNewWorkOrder(Employee creator);
	}
}