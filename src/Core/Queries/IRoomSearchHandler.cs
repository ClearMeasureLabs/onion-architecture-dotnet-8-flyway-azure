namespace ClearMeasure.Bootcamp.Core.Queries;

public interface IRoomSearchHandler
{
    public string Handle(RoomSearchQuery query);
}