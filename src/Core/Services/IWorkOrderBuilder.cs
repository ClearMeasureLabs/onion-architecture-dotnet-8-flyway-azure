using Core.Model;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace Core.Services
{
	public interface IWorkOrderBuilder
	{
		WorkOrder CreateNewWorkOrder(Employee creator);
	}
}