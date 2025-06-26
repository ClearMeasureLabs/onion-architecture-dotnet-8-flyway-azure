using Microsoft.AspNetCore.Mvc;
using Core.Model;
using Core.Services;

namespace ProgrammingWithPalermo.ChurchBulletin.UI.Api.Controllers;

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
    public ActionResult<Employee> GetByUserName(string userName)
    {
        var employee = _employeeRepository.GetByUserName(userName);
        if (employee == null) return NotFound();
        return employee;
    }

    [HttpGet]
    public ActionResult<Employee[]> GetEmployees()
    {
        var employees = _employeeRepository.GetEmployees(EmployeeSpecification.All);
        return employees;
    }
}
