using Bunit;
using Core.Model;
using Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using UI.Shared.Pages;
using UIWasm.Models;
using TestContext = Bunit.TestContext;

namespace ProgrammingWithPalermo.ChurchBulletin.UnitTests.UI.Shared.Pages;

public class WorkOrderSearchTests
{
    [Test]
    public void ShouldLoadDropDownsInitiallyOnLoad()
    {
        using var ctx = new TestContext();

        // Arrange
        ctx.Services.AddSingleton<IEmployeeRepository>(new StubEmployeeRepository());
        ctx.Services.AddSingleton<IWorkOrderRepository>(new StubWorkOrderRepository());

        // Act
        var component = ctx.RenderComponent<WorkOrderSearch>();

        // Assert
        var creatorSelect = component.Find($"#{WorkOrderSearch.Elements.CreatorSelect}");
        var assigneeSelect = component.Find($"#{WorkOrderSearch.Elements.AssigneeSelect}");
        var statusSelect = component.Find($"#{WorkOrderSearch.Elements.StatusSelect}");

        creatorSelect.ShouldNotBeNull();
        assigneeSelect.ShouldNotBeNull();
        statusSelect.ShouldNotBeNull();

        // Verify user options are loaded (3 employees + "All" option = 4 options)
        var creatorOptions = creatorSelect.QuerySelectorAll("option");
        creatorOptions.Length.ShouldBe(4);
        creatorOptions[0].TextContent.ShouldBe("All");

        var assigneeOptions = assigneeSelect.QuerySelectorAll("option");
        assigneeOptions.Length.ShouldBe(4);
        assigneeOptions[0].TextContent.ShouldBe("All");

        // Verify status options are loaded (5 statuses + "All" option = 6 options)
        var statusOptions = statusSelect.QuerySelectorAll("option");
        statusOptions.Length.ShouldBe(6);
        statusOptions[0].TextContent.ShouldBe("All");
    }

    [Test]
    public void ShouldNotInitiallyLoadAnyWorkOrders()
    {
        
    }

    private class StubEmployeeRepository : IEmployeeRepository
    {
        public Task<Employee> GetByUserNameAsync(string? userName)
        {
            var employee = new Employee(userName!, "Test", "User", "test@example.com");
            return Task.FromResult(employee);
        }

        public Task<Employee[]> GetEmployeesAsync(EmployeeSpecification spec)
        {
            var employees = new[]
            {
                new Employee("jpalermo", "Jeffrey", "Palermo", "jeffrey@example.com"),
                new Employee("hsimpson", "Homer", "Simpson", "homer@example.com"),
                new Employee("mburns", "Montgomery", "Burns", "burns@example.com")
            };
            return Task.FromResult(employees);
        }
    }

    private class StubWorkOrderRepository : IWorkOrderRepository
    {
        public Task SaveAsync(WorkOrder workOrder)
        {
            return Task.CompletedTask;
        }

        public Task<WorkOrder?> GetWorkOrderAsync(string number)
        {
            return Task.FromResult<WorkOrder?>(null);
        }

        public Task<WorkOrder[]> GetWorkOrdersAsync(WorkOrderSearchSpecification specification)
        {
            return Task.FromResult(Array.Empty<WorkOrder>());
        }
    }
}