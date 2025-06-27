using Core.Model;

namespace Core.Services
{
	public interface IEmployeeRepository
	{
		Task<Employee> GetByUserNameAsync(string userName);
		Task<Employee[]> GetEmployeesAsync(EmployeeSpecification spec);
	}
}