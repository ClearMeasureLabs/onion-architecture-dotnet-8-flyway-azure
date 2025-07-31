using ClearMeasure.Bootcamp.Core.Services;

namespace ClearMeasure.Bootcamp.Core.Model.StateCommands;

public record StateCommandResult(string TransitionVerbPresentTense, WorkOrder WorkOrder, string DebugMessage)
{
}