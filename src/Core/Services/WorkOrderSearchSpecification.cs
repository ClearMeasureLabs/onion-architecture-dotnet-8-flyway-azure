using Core.Model;

namespace Core.Services
{
	public class WorkOrderSearchSpecification
	{
		private WorkOrderStatus _status;
		private Employee _assignee;
		private Employee _creator;

		public void MatchStatus(WorkOrderStatus status)
		{
			_status = status;
		}

		public void MatchAssignee(Employee assignee)
		{
			_assignee = assignee;
		}

		public void MatchCreator(Employee creator)
		{
			_creator = creator;
		}

		public WorkOrderStatus Status
		{
			get { return _status; }
		}

		public Employee Assignee
		{
			get { return _assignee; }
		}

		public Employee Creator
		{
			get { return _creator; }
		}
	}
}