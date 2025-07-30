using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace Core.Model
{
    public class AuditEntry : IEquatable<AuditEntry>
    {
        public AuditEntry()
        {
            Employee = null!;
            Date = DateTime.MinValue;
            ArchivedEmployeeName = null!;
            BeginStatus = WorkOrderStatus.None;
            EndStatus = WorkOrderStatus.None;
        }

        public AuditEntry(Employee employee, DateTime date, WorkOrderStatus beginStatus, WorkOrderStatus endStatus)
        {
            Employee = employee;
            Date = date;
            ArchivedEmployeeName = employee.GetFullName();
            BeginStatus = beginStatus;
            EndStatus = endStatus;
        }

        public Employee Employee { get; set; }

        public DateTime Date { get; set; }

        public string ArchivedEmployeeName { get; set; }

        public WorkOrderStatus BeginStatus { get; set; }

        public WorkOrderStatus EndStatus { get; set; }

        public virtual bool Equals(AuditEntry? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Equals(Employee, other.Employee) &&
                   TruncateToSeconds(Date) == TruncateToSeconds(other.Date) &&
                   ArchivedEmployeeName == other.ArchivedEmployeeName &&
                   Equals(BeginStatus, other.BeginStatus) &&
                   Equals(EndStatus, other.EndStatus);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AuditEntry)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Employee, TruncateToSeconds(Date), ArchivedEmployeeName, BeginStatus, EndStatus);
        }

        public static bool operator ==(AuditEntry? left, AuditEntry? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(AuditEntry? left, AuditEntry? right)
        {
            return !Equals(left, right);
        }

        private static DateTime TruncateToSeconds(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day,
                               dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
        }
    }
}