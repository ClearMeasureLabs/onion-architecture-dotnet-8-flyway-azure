using ClearMeasure.Bootcamp.Core.Queries;

namespace LlmGateway
{
    public class RoomSearchHandler : IRoomSearchHandler
    {
        public string Handle(RoomSearchQuery query)
        {
            return "Room 205";
        }
    }
}
