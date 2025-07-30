using ClearMeasure.Bootcamp.Core;
using ClearMeasure.Bootcamp.Core.Model;
using DataAccess.Mappings;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace ClearMeasure.Bootcamp.DataAccess.Mappings;

public class DataContext : DbContext
{
    private readonly IDatabaseConfiguration _config;

    public DataContext(IDatabaseConfiguration config)
    {
        _config = config;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseSqlServer(_config.GetConnectionString());
        optionsBuilder.AddInterceptors(new AuditEntrySequenceInterceptor());


        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        new ChurchBulletinMap().Map(modelBuilder);
        new WorkOrderMap().Map(modelBuilder);
        new EmployeeMap().Map(modelBuilder);
        new RoleMap().Map(modelBuilder);
    }

    public override string ToString()
    {
        return base.ToString() + "-" + GetHashCode();
    }
}