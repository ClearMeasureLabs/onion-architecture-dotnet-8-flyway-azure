using Bunit;
using Core.Model;
using Core.Services;
using Microsoft.AspNetCore.Components;
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
    public void ShouldLoadWorkOrderTableWithAllFiltersSetToAllOnInitialLoad()
    {
        using var ctx = new TestContext();

        // Arrange
        var stubWorkOrderRepo = new StubWorkOrderRepository();
        ctx.Services.AddSingleton<IEmployeeRepository>(new StubEmployeeRepository());
        ctx.Services.AddSingleton<IWorkOrderRepository>(stubWorkOrderRepo);

        // Act
        var component = ctx.RenderComponent<WorkOrderSearch>();

        // Assert
        stubWorkOrderRepo.SearchWasCalled.ShouldBeTrue();
        stubWorkOrderRepo.LastSpecification!.Creator.ShouldBeNull();
        stubWorkOrderRepo.LastSpecification.Assignee.ShouldBeNull();
        stubWorkOrderRepo.LastSpecification.Status.ShouldBeNull();

        var workOrderTable = component.Find(".grid-data");
        workOrderTable.ShouldNotBeNull();
        
        var workOrderRows = workOrderTable.QuerySelectorAll("tbody tr");
        workOrderRows.Length.ShouldBe(2);
    }

    [Test]
    public void ShouldLoadWorkOrderTableWithCreatorFilterOnInitialLoad()
    {
        using var ctx = new TestContext();

        // Arrange
        var stubWorkOrderRepo = new StubWorkOrderRepository();
        ctx.Services.AddSingleton<IEmployeeRepository>(new StubEmployeeRepository());
        ctx.Services.AddSingleton<IWorkOrderRepository>(stubWorkOrderRepo);

        var navigationManager = ctx.Services.GetRequiredService<NavigationManager>();
        var uri = navigationManager.GetUriWithQueryParameter("Creator", "somename");
        navigationManager.NavigateTo(uri);

        // Act
        var component = ctx.RenderComponent<WorkOrderSearch>();

        // Assert
        stubWorkOrderRepo.SearchWasCalled.ShouldBeTrue();
        stubWorkOrderRepo.LastSpecification.ShouldNotBeNull();
        stubWorkOrderRepo.LastSpecification.Creator!.UserName.ShouldBe("somename");
        stubWorkOrderRepo.LastSpecification.Assignee.ShouldBeNull();
        stubWorkOrderRepo.LastSpecification.Status.ShouldBeNull();

        var workOrderTable = component.Find(".grid-data");
        var workOrderRows = workOrderTable.QuerySelectorAll("tbody tr");
        workOrderRows.Length.ShouldBe(2);
    }

    [Test]
    public void ShouldLoadWorkOrderTableWithAssigneeFilterOnInitialLoad()
    {
        using var ctx = new TestContext();

        // Arrange
        var stubWorkOrderRepo = new StubWorkOrderRepository();
        ctx.Services.AddSingleton<IEmployeeRepository>(new StubEmployeeRepository());
        ctx.Services.AddSingleton<IWorkOrderRepository>(stubWorkOrderRepo);

        var navigationManager = ctx.Services.GetRequiredService<NavigationManager>();
        var uri = navigationManager.GetUriWithQueryParameter("Assignee", "somename");
        navigationManager.NavigateTo(uri);

        // Act
        var component = ctx.RenderComponent<WorkOrderSearch>();

        // Assert
        stubWorkOrderRepo.SearchWasCalled.ShouldBeTrue();
        stubWorkOrderRepo.LastSpecification!.Creator.ShouldBeNull();
        stubWorkOrderRepo.LastSpecification.Assignee.ShouldNotBeNull();
        stubWorkOrderRepo.LastSpecification.Assignee.UserName.ShouldBe("somename");
        stubWorkOrderRepo.LastSpecification.Status.ShouldBeNull();

        var workOrderTable = component.Find(".grid-data");
        var workOrderRows = workOrderTable.QuerySelectorAll("tbody tr");
        workOrderRows.Length.ShouldBe(2);
    }

    [Test]
    public void ShouldLoadWorkOrderTableWithStatusFilterOnInitialLoad()
    {
        using var ctx = new TestContext();

        // Arrange
        var stubWorkOrderRepo = new StubWorkOrderRepository();
        ctx.Services.AddSingleton<IEmployeeRepository>(new StubEmployeeRepository());
        ctx.Services.AddSingleton<IWorkOrderRepository>(stubWorkOrderRepo);

        var navigationManager = ctx.Services.GetRequiredService<NavigationManager>();
        var uri = navigationManager.GetUriWithQueryParameter("Status", WorkOrderStatus.Assigned.Key);
        navigationManager.NavigateTo(uri);

        // Act
        var component = ctx.RenderComponent<WorkOrderSearch>();

        // Assert
        stubWorkOrderRepo.SearchWasCalled.ShouldBeTrue();
        stubWorkOrderRepo.LastSpecification!.Creator.ShouldBeNull();
        stubWorkOrderRepo.LastSpecification.Assignee.ShouldBeNull();
        stubWorkOrderRepo.LastSpecification.Status.ShouldNotBeNull();
        stubWorkOrderRepo.LastSpecification.Status?.ShouldBe(WorkOrderStatus.Assigned);

        var workOrderTable = component.Find(".grid-data");
        var workOrderRows = workOrderTable.QuerySelectorAll("tbody tr");
        workOrderRows.Length.ShouldBe(2);
    }

    [Test]
    public void AfterInitialLoadSelectingAllThreeOptionsShouldLoadWorkOrders()
    {
        using var ctx = new TestContext();

        // Arrange
        var stubWorkOrderRepo = new StubWorkOrderRepository();
        ctx.Services.AddSingleton<IEmployeeRepository>(new StubEmployeeRepository());
        ctx.Services.AddSingleton<IWorkOrderRepository>(stubWorkOrderRepo);

        var component = ctx.RenderComponent<WorkOrderSearch>();

        // Act
        var creatorSelect = component.Find($"#{WorkOrderSearch.Elements.CreatorSelect}");
        var assigneeSelect = component.Find($"#{WorkOrderSearch.Elements.AssigneeSelect}");
        var statusSelect = component.Find($"#{WorkOrderSearch.Elements.StatusSelect}");

        creatorSelect.Change("jpalermo");
        assigneeSelect.Change("hsimpson");
        statusSelect.Change(WorkOrderStatus.InProgress.Key);

        var searchButton = component.Find($"#{WorkOrderSearch.Elements.SearchButton}");
        searchButton.Click();

        // Assert
        stubWorkOrderRepo.SearchWasCalled.ShouldBeTrue();
        stubWorkOrderRepo.LastSpecification.ShouldNotBeNull();
        stubWorkOrderRepo.LastSpecification.Creator!.UserName.ShouldBe("jpalermo");
        stubWorkOrderRepo.LastSpecification.Assignee!.UserName.ShouldBe("hsimpson");
        stubWorkOrderRepo.LastSpecification.Status.ShouldBe(WorkOrderStatus.InProgress);

        var workOrderTable = component.Find(".grid-data");
        workOrderTable.ShouldNotBeNull();

        var workOrderRows = workOrderTable.QuerySelectorAll("tbody tr");
        workOrderRows.Length.ShouldBe(2);
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
        public bool SearchWasCalled { get; private set; }
        public WorkOrderSearchSpecification? LastSpecification { get; private set; }

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
            SearchWasCalled = true;
            LastSpecification = specification;

            var workOrders = new[]
            {
                new WorkOrder
                {
                    Number = "WO-001",
                    Title = "Fix broken door",
                    Status = WorkOrderStatus.Draft,
                    Creator = new Employee("jpalermo", "Jeffrey", "Palermo", "jeffrey@example.com"),
                    Assignee = new Employee("hsimpson", "Homer", "Simpson", "homer@example.com")
                },
                new WorkOrder
                {
                    Number = "WO-002", 
                    Title = "Replace light bulb",
                    Status = WorkOrderStatus.Assigned,
                    Creator = new Employee("mburns", "Montgomery", "Burns", "burns@example.com"),
                    Assignee = new Employee("jpalermo", "Jeffrey", "Palermo", "jeffrey@example.com")
                }
            };
            
            return Task.FromResult(workOrders);
        }
    }
}