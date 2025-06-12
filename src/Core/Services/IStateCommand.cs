using Core.Model;

namespace Core.Services
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