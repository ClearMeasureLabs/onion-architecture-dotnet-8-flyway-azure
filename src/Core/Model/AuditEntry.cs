using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Model
{
    public class AuditEntry
    {
        private Employee _employee;
        private DateTime _date;
        private string _archivedEmployeeName;
        private WorkOrderStatus _beginStatus;
        private WorkOrderStatus _endStatus;

        public AuditEntry()
        {
        }

        public AuditEntry(Employee employee, DateTime date, WorkOrderStatus beginStatus, WorkOrderStatus endStatus)
        {
            _employee = employee;
            _date = date;
            _archivedEmployeeName = employee.GetFullName();
            _beginStatus = beginStatus;
            _endStatus = endStatus;
        }

        public virtual Employee Employee
        {
            get { return _employee; }
            set { _employee = value; }
        }

        public virtual DateTime Date
        {
            get { return _date; }
            set { _date = value; }
        }

        public virtual string ArchivedEmployeeName
        {
            get { return _archivedEmployeeName; }
            set { _archivedEmployeeName = value; }
        }

        public virtual WorkOrderStatus BeginStatus
        {
            get { return _beginStatus; }
            set { _beginStatus = value; }
        }

        public virtual WorkOrderStatus  EndStatus
        {
            get { return _endStatus; }
            set { _endStatus = value; }
        }
    }
}
          