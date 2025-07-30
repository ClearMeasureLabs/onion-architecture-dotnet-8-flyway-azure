using Microsoft.AspNetCore.Components;
using ClearMeasure.Bootcamp.Core;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Queries;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.UI.Shared.Models;
using Palermo.BlazorMvc;

namespace ClearMeasure.Bootcamp.UI.Shared.Pages;

[Route("/workorder/search")]
public class WorkOrderSearchController : ControllerComponentBase<WorkOrderSearch>
{
    [Inject] public IEmployeeRepository EmployeeRepository { get; set; } = null!;
    [Inject] public IBus AppBus { get; set; } = null!;

    [SupplyParameterFromQuery] public string? Creator { get; set; }
    [SupplyParameterFromQuery] public string? Assignee { get; set; }
    [SupplyParameterFromQuery] public string? Status { get; set; }

    protected override void OnViewInitialized()
    {
        // Initialize the view model
        View.Model = new WorkOrderSearchModel();
        
        // Set up event handlers
        View.OnSearch = HandleSearch;

        // Initialize data asynchronously
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        // Initialize dropdown options
        await LoadUserOptions();
        LoadStatusOptions();

        // Apply any query parameters
        if (!string.IsNullOrEmpty(Creator))
            View.Model.Filters.Creator = Creator;

        if (!string.IsNullOrEmpty(Assignee))
            View.Model.Filters.Assignee = Assignee;

        if (!string.IsNullOrEmpty(Status))
            View.Model.Filters.Status = Status;

        // Perform initial search
        await SearchWorkOrders();
    }

    private async Task HandleSearch()
    {
        await SearchWorkOrders();
    }

    private async Task SearchWorkOrders()
    {
        var creator = !string.IsNullOrWhiteSpace(View.Model.Filters.Creator)
            ? await EmployeeRepository.GetByUserNameAsync(View.Model.Filters.Creator)
            : null;

        var assignee = !string.IsNullOrWhiteSpace(View.Model.Filters.Assignee)
            ? await EmployeeRepository.GetByUserNameAsync(View.Model.Filters.Assignee)
            : null;

        var status = !string.IsNullOrWhiteSpace(View.Model.Filters.Status)
            ? WorkOrderStatus.FromKey(View.Model.Filters.Status)
            : null;

        var specification = new WorkOrderSpecificationQuery();
        specification.MatchCreator(creator);
        specification.MatchAssignee(assignee);
        specification.MatchStatus(status);

        View.Model.Results = await AppBus.Send(specification);
        StateHasChanged();
    }

    private async Task LoadUserOptions()
    {
        var employees = await EmployeeRepository.GetEmployeesAsync(EmployeeSpecification.All);
        View.UserOptions = employees.Select(e => new WorkOrderSearch.SelectListItem(e.UserName, e.GetFullName())).ToList();
    }

    private void LoadStatusOptions()
    {
        View.StatusOptions = WorkOrderStatus.GetAllItems().Select(s => new WorkOrderSearch.SelectListItem(s.Key, s.FriendlyName)).ToList();
    }
}