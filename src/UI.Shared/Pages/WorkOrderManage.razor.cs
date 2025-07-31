using BlazorApplicationInsights.Models.Context;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Queries;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Services.Impl;
using ClearMeasure.Bootcamp.UI.Shared.Models;
using Microsoft.AspNetCore.Components;

namespace ClearMeasure.Bootcamp.UI.Shared.Pages;

[Route("/workorder/manage/{id?}")]
public partial class WorkOrderManage : AppComponentBase
{
    [Inject] public IWorkOrderBuilder? WorkOrderBuilder { get; set; }
    [Inject] public IWorkOrderRepository? WorkOrderRepository { get; set; }
    [Inject] public IUserSession? UserSession { get; set; }
    [Inject] private IWorkflowFacilitator? WorkflowFacilitator { get; set; }
    [Inject] private NavigationManager? NavigationManager { get; set; }
    
    public WorkOrderManageModel Model { get; set; } = new WorkOrderManageModel();
    public List<SelectListItem> UserOptions { get; set; } = new List<SelectListItem>();
    public IEnumerable<IStateCommand> ValidCommands { get; set; } = new List<IStateCommand>();
    public string? SelectedCommand { get; set; }

    [Parameter]
    public string? Id { get; set; }

    [SupplyParameterFromQuery]
    public EditMode Mode { get; set; } = EditMode.Edit;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserOptions();
        await LoadWorkOrder();
    }

    private async Task LoadWorkOrder()
    {
        Employee currentUser = (await UserSession!.GetCurrentUserAsync())!;
        WorkOrder workOrder;

        if (Mode == EditMode.New)
        {
            workOrder = WorkOrderBuilder!.CreateNewWorkOrder(currentUser);
            if (!string.IsNullOrEmpty(Id))
                workOrder.Number = Id;
        }
        else
        {
            workOrder = (await WorkOrderRepository!.GetWorkOrderAsync(Id!))!;
        }

        Model = CreateViewModel(Mode, workOrder);
        Model.IsReadOnly = !(WorkflowFacilitator!.GetValidStateCommands(workOrder, currentUser)).Any();
        ValidCommands = WorkflowFacilitator.GetValidStateCommands(workOrder, currentUser);
    }

    private WorkOrderManageModel CreateViewModel(EditMode mode, WorkOrder workOrder)
    {
        return new WorkOrderManageModel
        {
            WorkOrder = workOrder,
            Mode = mode,
            WorkOrderNumber = workOrder.Number,
            Status = workOrder.Status!.FriendlyName,
            CreatorUserName = workOrder.Creator!.UserName,
            CreatorFullName = workOrder.Creator!.GetFullName(),
            AssignedToUserName = workOrder.Assignee?.UserName,
            Title = workOrder.Title,
            Description = workOrder.Description,
            CreatedDate = workOrder.CreatedDate.ToString(),
            AssignedDate = workOrder.AssignedDate?.ToString(),
            CompletedDate = workOrder.CompletedDate?.ToString()
        };
    }

    private async Task HandleSubmit()
    {
        Employee currentUser = (await UserSession!.GetCurrentUserAsync())!;
        WorkOrder workOrder;

        if (Model.Mode == EditMode.New)
            workOrder = WorkOrderBuilder!.CreateNewWorkOrder(currentUser);
        else
            workOrder = (await WorkOrderRepository!.GetWorkOrderAsync(Model.WorkOrderNumber!))!;

        var assignee = await Bus.Send(new EmployeeByUserNameQuery(Model.AssignedToUserName));
        var creator = await Bus.Send(new EmployeeByUserNameQuery(Model.CreatorUserName));

        workOrder.Number = Model.WorkOrderNumber;
        workOrder.Creator = creator;
        workOrder.Assignee = assignee;
        workOrder.Title = Model.Title;
        workOrder.Description = Model.Description;
        workOrder.RoomNumber = Model.RoomNumber;

        IStateCommand[] commands = WorkflowFacilitator!.GetValidStateCommands(workOrder, currentUser);
        IStateCommand matchingCommand =
            Array.Find(commands, obj => obj.Matches(SelectedCommand!))!;

        var result = await Bus.Send(matchingCommand);
        
        NavigationManager!.NavigateTo("/workorder/search");
    }

    private async Task LoadUserOptions()
    {
        // In a real implementation, this would load all employees from the repository
        var employees = await Bus.Send(new EmployeeGetAllQuery());
        UserOptions = employees.Select(e => new SelectListItem
        {
            Value = e.UserName,
            Text = e.GetFullName()
        }).ToList();
    }

    public class SelectListItem
    {
        public required string Value { get; set; }
        public required string Text { get; set; }
    }
}