namespace Core.Model
{
    public class AuditEntry
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
    }
}