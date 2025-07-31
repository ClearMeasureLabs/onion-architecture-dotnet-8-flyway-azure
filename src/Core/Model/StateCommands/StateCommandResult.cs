namespace ClearMeasure.Bootcamp.Core.Model.StateCommands;

public class StateCommandResult(StateCommandBase command, WorkOrder order, string debugMessage)
{
    public StateCommandBase Command { get; } = command;
    public WorkOrder Order { get; } = order;
    public string DebugMessage { get; } = debugMessage;
}