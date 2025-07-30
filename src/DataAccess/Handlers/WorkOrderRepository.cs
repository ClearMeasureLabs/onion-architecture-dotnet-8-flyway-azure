using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearMeasure.Bootcamp.DataAccess.Mappings;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace ClearMeasure.Bootcamp.DataAccess.Handlers
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly DataContext _context;

        public WorkOrderRepository(DataContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(WorkOrder workOrder)
        {
            _context.Attach(workOrder);
            await _context.SaveChangesAsync();
        }

        public async Task<WorkOrder?> GetWorkOrderAsync(string number)
        {
            return await _context.Set<WorkOrder>()
                .Include(wo => wo.AuditEntries)
                .SingleOrDefaultAsync(wo => wo.Number == number);
        }

        public async Task<WorkOrder[]> GetWorkOrdersAsync(WorkOrderSearchSpecification specification)
        {
            IQueryable<WorkOrder> query = _context.Set<WorkOrder>();

            if (specification.Assignee != null)
            {
                query = query.Where(wo => wo.Assignee == specification.Assignee);
            }

            if (specification.Creator != null)
            {
                query = query.Where(wo => wo.Creator == specification.Creator);
            }

            if (specification.Status != null)
            {
                query = query.Where(wo => wo.Status == specification.Status);
            }

            return await query.ToArrayAsync();
        }
    }
}