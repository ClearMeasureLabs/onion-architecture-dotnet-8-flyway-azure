using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.UI.Api.Controllers;

[ApiController]
[Route("employee")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    public EmployeeController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [HttpGet("by-username/{userName}")]
    public async Task<ActionResult<Employee>> GetByUserName(string? userName)
    {
        var employee = await _employeeRepository.GetByUserNameAsync(userName);
        if (employee == null) return NotFound();
        return employee;
    }

    [HttpGet]
    public async Task<ActionResult<Employee[]>> GetEmployees()
    {
        var employees = await _employeeRepository.GetEmployeesAsync(EmployeeSpecification.All);
        return employees;
    }
}
