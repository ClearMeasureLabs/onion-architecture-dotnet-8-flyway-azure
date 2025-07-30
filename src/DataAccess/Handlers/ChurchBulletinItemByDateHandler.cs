using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Queries;
using ClearMeasure.Bootcamp.DataAccess.Mappings;

namespace ClearMeasure.Bootcamp.DataAccess.Handlers;

public class ChurchBulletinItemByDateHandler : IChurchBulletinItemByDateHandler
{
    private readonly DataContext _context;

    public ChurchBulletinItemByDateHandler(DataContext context)
    {
        _context = context;
    }
    public IEnumerable<ChurchBulletinItem> Handle(ChurchBulletinItemByDateAndTimeQuery query)
    {
        var items = _context.Set<ChurchBulletinItem>()
            .Where(item => item.Date == query.TargetDate).AsEnumerable();
        return items;
    }
}