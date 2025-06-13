using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;

namespace DataAccess.Mappings
{
    public class WorkOrderMap : IEntityFrameworkMapping
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var statusConverter = new WorkOrderStatusConverter();
            
            modelBuilder.Entity<WorkOrder>(entity =>
            {
                entity.ToTable("WorkOrder", "dbo");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(Guid.Empty);

                entity.Property(e => e.Number).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.RoomNumber).HasMaxLength(50);

                // Configure relationships
                entity.HasOne(e => e.Creator)
                      .WithMany()
                      .HasForeignKey("CreatorId")
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Assignee)
                      .WithMany()
                      .HasForeignKey("AssigneeId")
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure Status with converter
                entity.Property(e => e.Status)
                      .HasConversion(statusConverter)
                      .HasMaxLength(3);

                // Configure AuditEntries collection with index-based ordering
                entity.HasMany(e => e.AuditEntries)
                      .WithOne()
                      .HasForeignKey("WorkOrderId")
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(e => e.AuditEntries).AutoInclude();
            });

            modelBuilder.Entity<AuditEntry>(entity =>
            {
                entity.ToTable("AuditEntry", "dbo");

                // Configure the sequence property for list order
                entity.Property<int>("Sequence")
                    .IsRequired();
                
                // Configure the composite key (WorkOrderId, Sequence)
                entity.HasKey("WorkOrderId", "Sequence");

                // Configure properties
                entity.Property(e => e.ArchivedEmployeeName).HasMaxLength(50);
                entity.Property(e => e.Date).IsRequired();

                // Configure Employee relationship
                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey("EmployeeId")
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure conversion for the WorkOrderStatus properties
                entity.Property(e => e.BeginStatus)
                      .HasConversion(statusConverter)
                      .HasMaxLength(3)
                      .HasColumnType("char(3)");

                entity.Property(e => e.EndStatus)
                      .HasConversion(statusConverter)
                      .HasMaxLength(3)
                      .HasColumnType("char(3)");
            });
        }
    }
}