using ClearMeasure.Bootcamp.Core.Model;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace ClearMeasure.Bootcamp.DataAccess.Mappings;

public class AuditEntrySequenceGenerator : ValueGenerator<int>
{
    public override bool GeneratesTemporaryValues => false;

    public override int Next(EntityEntry entry)
    {
        var auditEntry = entry.Entity as AuditEntry;
        return auditEntry?.GetHashCode() ?? 0;
    }
}