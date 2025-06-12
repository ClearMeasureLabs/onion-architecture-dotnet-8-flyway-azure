using Core.Model;

namespace Core.Services.Impl
{
	public class WorkOrderBuilder : IWorkOrderBuilder
	{
		private readonly IWorkOrderNumberGenerator _numberGenerator;
		private readonly ICalendar _calendar;

		public WorkOrderBuilder(IWorkOrderNumberGenerator numberGenerator, ICalendar calendar)
		{
			_numberGenerator = numberGenerator;
			_calendar = calendar;
		}

		public WorkOrder CreateNewWorkOrder(Employee creator)
		{
			WorkOrder workOrder = new WorkOrder();
			workOrder.Number = _numberGenerator.GenerateNumber();
			workOrder.Creator = creator;
			workOrder.Status = WorkOrderStatus.Draft;
			return workOrder;
		}
	}
}