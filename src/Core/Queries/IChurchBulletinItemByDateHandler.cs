using ClearMeasure.Bootcamp.Core.Model;

namespace ClearMeasure.Bootcamp.Core.Queries;

public interface IChurchBulletinItemByDateHandler
{
    IEnumerable<ChurchBulletinItem> Handle(ChurchBulletinItemByDateAndTimeQuery query);
}