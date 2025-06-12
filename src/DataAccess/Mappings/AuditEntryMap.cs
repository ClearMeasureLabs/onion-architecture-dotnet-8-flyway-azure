using Core.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;

namespace DataAccess.Mappings
{
    public class AuditEntryMap : IEntityFrameworkMapping
    {
        public void Map(ModelBuilder modelBuilder)
        {
            var statusConverter = new WorkOrderStatusConverter();
            
            modelBuilder.Entity<AuditEntry>(entity =>
            {
                entity.ToTable("AuditEntry", "dbo");
                
                // Add shadow property for Id since AuditEntry doesn't have an Id
                entity.Property<Guid>("Id").IsRequired()
                    .ValueGeneratedOnAdd()
                    .HasDefaultValue(Guid.Empty);
                entity.HasKey("Id");

                entity.Property(e => e.ArchivedEmployeeName).HasMaxLength(200);
                entity.Property(e => e.Date).IsRequired();

                // Configure Employee relationship
                entity.HasOne(e => e.Employee)
                      .WithMany()
                      .HasForeignKey("EmployeeId")
                      .OnDelete(DeleteBehavior.Restrict);

                // Configure conversion for the WorkOrderStatus properties
                entity.Property(e => e.BeginStatus)
                      .HasConversion(statusConverter)
                      .HasMaxLength(3);

                entity.Property(e => e.EndStatus)
                      .HasConversion(statusConverter)
                      .HasMaxLength(3);
            });
        }
    }
}