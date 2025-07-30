using Core.Services;
using ProgrammingWithPalermo.ChurchBulletin.Core.Model;

namespace Core.Model.StateCommands
{
    public class AssignedToDraftForWithdrawCommand :StateCommandBase
    {
        public AssignedToDraftForWithdrawCommand(WorkOrder workOrder, Employee currentUser) : base(workOrder, currentUser)
        {
        }

        public override WorkOrderStatus GetBeginStatus()
        {
            return WorkOrderStatus.Assigned;
        }

        protected override WorkOrderStatus GetEndStatus()
        {
            return WorkOrderStatus.Draft;
        }

        protected override bool userCanExecute(Employee currentUser)
        {
            return currentUser == _workOrder.Creator;
        }

        public override string TransitionVerbPresentTense
        {
            get { return "Withdraw"; }
        }

        public override string TransitionVerbPastTense
        {
            get { return "Withdrawn"; }
        }

        protected override void preExecute(IStateCommandVisitor commandVisitor)
        {
        }

        protected override void postExecute(IStateCommandVisitor commandVisitor)
        {
            commandVisitor.EditWorkOrder(_workOrder);
        }
    }
}
