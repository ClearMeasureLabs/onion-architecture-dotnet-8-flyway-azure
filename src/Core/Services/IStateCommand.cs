using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Services
{
	public interface IStateCommand
	{
		bool IsValid();
		void Execute(IStateCommandVisitor commandVisitor, INotifier notifier);

		string TransitionVerbPresentTense { get; }
		bool Matches(string commandName);
		WorkOrderStatus GetBeginStatus();
	}
}