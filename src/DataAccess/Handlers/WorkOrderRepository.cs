using System.Collections.Generic;
using System.Linq;
using Core.Model;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;

namespace ProgrammingWithPalermo.ChurchBulletin.DataAccess.Handlers
{
    public class WorkOrderRepository : IWorkOrderRepository
    {
        private readonly DataContext _context;

        public WorkOrderRepository(DataContext context)
        {
            _context = context;
        }

        public void Save(WorkOrder workOrder)
        {
            _context.Set<WorkOrder>().Update(workOrder);
            _context.SaveChanges();
        }

        public WorkOrder GetWorkOrder(string number)
        {
            return _context.Set<WorkOrder>()
                .Include(wo => wo.AuditEntries)
                .SingleOrDefault(wo => wo.Number == number);
        }

        public WorkOrder[] GetWorkOrders(WorkOrderSearchSpecification specification)
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

            return query.ToArray();
        }
    }
}