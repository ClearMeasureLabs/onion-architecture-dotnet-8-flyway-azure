using Core.Model;

namespace Core.Services
{
	public interface IEmployeeRepository
	{
		Employee GetByUserName(string userName);
		Employee[] GetEmployees(EmployeeSpecification spec);
	}
}