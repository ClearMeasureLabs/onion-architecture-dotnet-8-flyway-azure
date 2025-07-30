using ClearMeasure.Bootcamp.Core.Model;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace ClearMeasure.Bootcamp.IntegrationTests.DataAccess.Mappings;

public class ChurchBulletinMappingTester
{
    [Test]
    public void ShouldMapChurchBulletin()
    {
        new DatabaseTester().Clean();
        var bulletin = new ChurchBulletinItem();
        bulletin.Name = "Worship service";
        bulletin.Place = "Sanctuary";
        bulletin.Date = new DateTime(2022, 1, 1);

        using (var context = TestHost.GetRequiredService<DbContext>())
        {
            context.Add(bulletin);
            context.SaveChanges();
        }

        ChurchBulletinItem rehydratedEntity;
        using (var context = TestHost.GetRequiredService<DbContext>())
        {
            rehydratedEntity = context.Set<ChurchBulletinItem>()
                .Single(b => b.Id == bulletin.Id);
        }

        rehydratedEntity.Id.ShouldBe(bulletin.Id);
        rehydratedEntity.ShouldBe(bulletin);
        rehydratedEntity.Name.ShouldBe(bulletin.Name);
        rehydratedEntity.Place.ShouldBe(bulletin.Place);
        rehydratedEntity.Date.ShouldBe(bulletin.Date);
    }
}