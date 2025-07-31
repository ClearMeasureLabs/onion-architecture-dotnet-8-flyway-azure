using ClearMeasure.Bootcamp.Core.Model;
using Microsoft.EntityFrameworkCore;

namespace ClearMeasure.Bootcamp.DataAccess.Mappings
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

                entity.Property(e => e.Number).IsRequired().HasMaxLength(5);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(4000);
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

                // Configure navigation properties for eager loading
                entity.Navigation(e => e.Creator).AutoInclude();
                entity.Navigation(e => e.Assignee).AutoInclude();

                // Configure Status with converter
                entity.Property(e => e.Status)
                      .HasConversion(statusConverter)
                      .HasMaxLength(3);

                // Configure AuditEntries collection with ordered mapping
                entity.HasMany(e => e.AuditEntries)
                      .WithOne()
                      .HasForeignKey("WorkOrderId")
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Navigation(e => e.AuditEntries)
                      .AutoInclude()
                      .EnableLazyLoading(false);
                
                // Configure ordering for AuditEntries collection
                entity.Metadata.FindNavigation(nameof(WorkOrder.AuditEntries))!
                      .SetPropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<AuditEntry>(entity =>
            {
                entity.ToTable("AuditEntry", "dbo");

                // Configure the sequence property as an ordered index column
                entity.Property<int>("Sequence")
                    .HasValueGenerator<AuditEntrySequenceGenerator>();
                
                // Configure the composite key (WorkOrderId, Sequence)
                entity.HasKey("WorkOrderId", "Sequence");

                // Add index for ordered queries and performance
                entity.HasIndex("WorkOrderId", "Sequence")
                      .HasDatabaseName("IX_AuditEntry_WorkOrderId_Sequence")
                      .IsUnique();

                // Configure properties to match database schema
                entity.Property(e => e.ArchivedEmployeeName)
                      .HasMaxLength(50)
                      .IsRequired(false); // nullable in database

                entity.Property(e => e.Date)
                      .HasColumnType("datetime")
                      .IsRequired(true); // nullable in database

                // Configure conversion for the WorkOrderStatus properties - nullable in database
                entity.Property(e => e.BeginStatus)
                      .HasConversion(statusConverter)
                      .HasMaxLength(3)
                      .HasColumnType("char(3)")
                      .IsRequired(true);

                entity.Property(e => e.EndStatus)
                      .HasConversion(statusConverter)
                      .HasMaxLength(3)
                      .HasColumnType("char(3)")
                      .IsRequired(true);
            });
        }
    }
}