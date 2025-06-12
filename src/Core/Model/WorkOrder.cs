using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Model;

namespace Core.Model
{
    public class WorkOrder
    {
        private Guid _id;
        private string _title = "";
        private string _description = "";
        private string _roomNumber;
        private WorkOrderStatus _status = WorkOrderStatus.Draft;
        private Employee _creator;
        private Employee _assignee;
        private string _number;
        private DateTime? _assignedDate;
        private DateTime? _createdDate;
        private DateTime? _completedDate;
        private IList<AuditEntry> _auditEntries;

        public WorkOrder()
        {
            _auditEntries = new List<AuditEntry>();
        }

        public IList<AuditEntry> AuditEntries
        {
            get { return _auditEntries; }
            set { _auditEntries = value; }
        }

        public virtual Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual string Title
    	{
    		get { return _title; }
    		set { _title = value; }
    	}

        public virtual string Description
		{
			get { return _description; }
			set { _description = getTruncatedString(value); }
		}

        public string RoomNumber
        {
            get { return _roomNumber; }
            set { _roomNumber = value; }
        }

        private string getTruncatedString(string value)
		{
            if (value == null)
                return string.Empty;
			int maxLength = Math.Min(4000, value.Length);
			return value.Substring(0, maxLength);
		}

        public virtual WorkOrderStatus Status
		{
			get { return _status; }
			set { _status = value; }
		}

        public virtual Employee Creator
        {
            get { return _creator; }
            set { _creator = value; }
        }

        public virtual Employee Assignee
        {
            get { return _assignee; }
            set { _assignee = value; }
        }

        public virtual string Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public virtual string FriendlyStatus
        {
            get { return getTextForStatus(); }
        }

        protected virtual string getTextForStatus()
        {
            return Status.ToString();
        }

        public override string ToString()
        {
            return "Work Order " + Number;
        }

        public virtual void ChangeStatus(WorkOrderStatus status)
        {
            Status = status;
        }

        public virtual void ChangeStatus(Employee employee, DateTime date, WorkOrderStatus status)
        {
            AuditEntry auditItem = new AuditEntry(employee, date, _status, status);
            AuditEntries.Add(auditItem);
            Status = status;
        }

        public string GetTweetMessage()
        {
            return "Work Order " + Number + " is now in Status "+ Status;
        }

       
        public DateTime? AssignedDate
          {
            get { return _assignedDate; }
            set { _assignedDate = value; }
          }

        public DateTime? CreatedDate
        {
            get { return _createdDate; }
            set { _createdDate = value; }
        }

        public DateTime? CompletedDate
        {
            get { return _completedDate; }
            set { _completedDate = value; }
        }

    }
}
