using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Services.Impl
{
	public class WorkOrderBuilder(IWorkOrderNumberGenerator numberGenerator, ICalendar calendar)
        : IWorkOrderBuilder
    {
        private readonly ICalendar _calendar = calendar;

        public WorkOrder CreateNewWorkOrder(Employee creator)
		{
			WorkOrder workOrder = new WorkOrder
            {
                Number = numberGenerator.GenerateNumber(),
                Creator = creator,
                Status = WorkOrderStatus.Draft
            };
            return workOrder;
		}
	}
}