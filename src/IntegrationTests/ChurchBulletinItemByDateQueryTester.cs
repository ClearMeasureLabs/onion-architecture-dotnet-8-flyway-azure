using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Queries;
using ClearMeasure.Bootcamp.DataAccess.Handlers;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace ClearMeasure.Bootcamp.IntegrationTests;

[TestFixture]
public class ChurchBulletinItemByDateQueryTester
{
    [Test]
    public void ShouldGetWithinDate()
    {
        EmptyDatabase();

        var item1 = new ChurchBulletinItem {Date = new DateTime(2000, 1, 1)};
        var item2 = new ChurchBulletinItem {Date = new DateTime(1999, 1, 1)};
        var item3 = new ChurchBulletinItem {Date = new DateTime(2001, 1, 1)};
        var item4 = new ChurchBulletinItem {Date = new DateTime(2000, 1, 1)};

        using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.AddRange(item1, item2, item3, item4);
            context.SaveChanges();
        }

        //arrange
        var query = new ChurchBulletinItemByDateAndTimeQuery(new DateTime(2000, 1, 1));
        IChurchBulletinItemByDateHandler handler = TestHost.GetRequiredService<ChurchBulletinItemByDateHandler>();

        //act
        IEnumerable<ChurchBulletinItem> items = handler.Handle(query).ToList();

        //assert
        items.Count().ShouldBe(2);
        items.ShouldContain(item1);
        items.ShouldContain(item4);
        items.ShouldNotContain(item2);
        items.ShouldNotContain(item3);
    }

    private void EmptyDatabase()
    {
        new DatabaseEmptier(TestHost.GetRequiredService<DbContext>().Database).DeleteAllData();
    }
}