using System;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.Core.Services.Impl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands
{
	public abstract class StateCommandBase : IStateCommand
	{
		protected WorkOrder _workOrder;
		protected Employee _currentUser;
        protected readonly ILogger<StateCommandBase> _logger;

	    protected StateCommandBase(WorkOrder workOrder, Employee currentUser)
		{
			_workOrder = workOrder;
			_currentUser = currentUser;
            _logger = NullLogger<StateCommandBase>.Instance;
		}

		public abstract WorkOrderStatus GetBeginStatus();
		protected abstract WorkOrderStatus GetEndStatus();
		protected abstract bool userCanExecute(Employee currentUser);
		public abstract string TransitionVerbPresentTense { get; }
		public abstract string TransitionVerbPastTense { get; }

		protected virtual void preExecute(IStateCommandVisitor commandVisitor)
		{
		}

		protected abstract void postExecute(IStateCommandVisitor commandVisitor);


	    protected virtual void sendChangeStateNotification(INotifier notifier)
	    {
           notifier.SendChangeStateNotification(string.Format("Work order {0} changed from {1} to {2}",_workOrder.Number, GetBeginStatus(), GetEndStatus()));          
	    }

	    protected virtual void sendAssignedNotification(INotifier notifier){}

	    public bool IsValid()
		{
			bool beginStatusMatches = _workOrder.Status == GetBeginStatus();
			bool currentUserIsCorrectRole = userCanExecute(_currentUser);
			return beginStatusMatches && currentUserIsCorrectRole;
		}

        public bool ShouldSendAssignmentNotification()
        {
            return _workOrder.Status == WorkOrderStatus.Assigned;
        }

		public void Execute(IStateCommandVisitor commandVisitor, INotifier notifier)
		{   
            _logger.LogInformation("Executing");
			preExecute(commandVisitor);
			string currentUserFullName = _currentUser.GetFullName();
            _workOrder.ChangeStatus(_currentUser, DateTime.Now,  this.GetEndStatus());
   
		    commandVisitor.SaveWorkOrder(_workOrder);

			string loweredTransitionVerb = TransitionVerbPastTense.ToLower();
			string workOrderNumber = _workOrder.Number;
			string message = string.Format("You have {0} work order {1}", loweredTransitionVerb, workOrderNumber);
			commandVisitor.SendMessage(message);
		    string debugMessage = string.Format("{0} has {1} work order {2}", currentUserFullName, loweredTransitionVerb,
		                                        workOrderNumber);
            _logger.LogDebug(debugMessage);
            sendChangeStateNotification(notifier);
            sendAssignedNotification(notifier);
			postExecute(commandVisitor);

            _logger.LogInformation("Executed");
		}

		public bool Matches(string commandName)
		{
			return TransitionVerbPresentTense == commandName;
		}
	}
}
