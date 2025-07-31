using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;

namespace ClearMeasure.Bootcamp.DataAccess.Handlers;

public class WorkOrderRepository(DataContext context) : IWorkOrderRepository
{
    public async Task SaveAsync(WorkOrder workOrder)
    {
        context.Attach(workOrder);
        await context.SaveChangesAsync();
    }

    public async Task<WorkOrder?> GetWorkOrderAsync(string number)
    {
        return await context.Set<WorkOrder>()
            .Include(wo => wo.AuditEntries)
            .SingleOrDefaultAsync(wo => wo.Number == number);
    }

    public async Task<WorkOrder[]> GetWorkOrdersAsync(WorkOrderSearchSpecification specification)
    {
        IQueryable<WorkOrder> query = context.Set<WorkOrder>();

        if (specification.Assignee != null) query = query.Where(wo => wo.Assignee == specification.Assignee);

        if (specification.Creator != null) query = query.Where(wo => wo.Creator == specification.Creator);

        if (specification.Status != null) query = query.Where(wo => wo.Status == specification.Status);

        return await query.ToArrayAsync();
    }
}