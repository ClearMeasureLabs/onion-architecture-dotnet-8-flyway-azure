using Microsoft.AspNetCore.Components;
using ClearMeasure.Bootcamp.Core;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Queries;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.UI.Shared.Models;
using Microsoft.AspNetCore.Authorization;

namespace ClearMeasure.Bootcamp.UI.Shared.Pages;

[Route("/workorder/search")]
[Authorize]
public partial class WorkOrderSearch : AppComponentBase
{
    [Inject] public IEmployeeRepository EmployeeRepository { get; set; } = null!;
    [SupplyParameterFromQuery] public string? Creator { get; set; }
    [SupplyParameterFromQuery] public string? Assignee { get; set; }
    [SupplyParameterFromQuery] public string? Status { get; set; }

    protected override void OnParametersSet()
    {
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        OnSearch = HandleSearch;

        var employees = await EmployeeRepository.GetEmployeesAsync(EmployeeSpecification.All);
        UserOptions = employees.Select(e => new WorkOrderSearch.SelectListItem(e.UserName, e.GetFullName())).ToList();
        StatusOptions = WorkOrderStatus.GetAllItems().Select(s => new WorkOrderSearch.SelectListItem(s.Key, s.FriendlyName)).ToList();
        Model = new WorkOrderSearchModel();
        
        // Apply any query parameters
        if (!string.IsNullOrEmpty(Creator))
            Model.Filters.Creator = Creator;

        if (!string.IsNullOrEmpty(Assignee))
            Model.Filters.Assignee = Assignee;

        if (!string.IsNullOrEmpty(Status))
            Model.Filters.Status = Status;

        // Perform initial search
        await SearchWorkOrders();
    }

    private async Task SearchWorkOrders()
    {
        var creator = !string.IsNullOrWhiteSpace(Model.Filters.Creator)
            ? await EmployeeRepository.GetByUserNameAsync(Model.Filters.Creator)
            : null;

        var assignee = !string.IsNullOrWhiteSpace(Model.Filters.Assignee)
            ? await EmployeeRepository.GetByUserNameAsync(Model.Filters.Assignee)
            : null;

        var status = !string.IsNullOrWhiteSpace(Model.Filters.Status)
            ? WorkOrderStatus.FromKey(Model.Filters.Status)
            : null;

        var specification = new WorkOrderSpecificationQuery();
        specification.MatchCreator(creator);
        specification.MatchAssignee(assignee);
        specification.MatchStatus(status);

        Model.Results = await Bus.Send(specification);
        StateHasChanged();
    }

    private async Task HandleSearch()
    {
        await SearchWorkOrders();
    }
}