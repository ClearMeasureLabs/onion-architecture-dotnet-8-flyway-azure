using ClearMeasure.Bootcamp.Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ClearMeasure.Bootcamp.DataAccess.Mappings;

public class AuditEntrySequenceInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        AssignAuditSequences(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    private void AssignAuditSequences(DbContext? context)
    {
        if (context == null) return;

        var entityEntries = context.ChangeTracker.Entries<WorkOrder>();
        var workOrders = entityEntries
            .Select(e => e.Entity);

        foreach (var workOrder in workOrders)
        {
            for (int i = 0; i < workOrder.AuditEntries.Count; i++)
            {
                var auditEntry = workOrder.AuditEntries[i];
                var auditEntryEntry = context.Entry(auditEntry);
                auditEntryEntry.Property("Sequence").CurrentValue = i;
            }
        }
    }
}