using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.Core.Services
{
	public interface IEmployeeRepository
	{
		Task<Employee> GetByUserNameAsync(string? userName);
		Task<Employee[]> GetEmployeesAsync(EmployeeSpecification spec);
	}
}