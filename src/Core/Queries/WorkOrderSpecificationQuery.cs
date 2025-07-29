using Core.Model;
using MediatR;

namespace ProgrammingWithPalermo.ChurchBulletin.Core.Queries;

public record WorkOrderSpecificationQuery : IRequest<WorkOrder[]>, IRemotableRequest
{
    public void MatchStatus(WorkOrderStatus? status)
    {
        Status = status;
    }

    public void MatchAssignee(Employee? assignee)
    {
        Assignee = assignee;
    }

    public void MatchCreator(Employee? creator)
    {
        Creator = creator;
    }

    public WorkOrderStatus? Status { get; set; } = null;

    public Employee? Assignee { get; set; } = null;

    public Employee? Creator { get; set; } = null;
}