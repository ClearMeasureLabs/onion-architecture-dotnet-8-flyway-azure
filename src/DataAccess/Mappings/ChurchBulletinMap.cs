using ClearMeasure.Bootcamp.Core.Model;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClearMeasure.Bootcamp.DataAccess.Mappings;

public class ChurchBulletinMap : EntityMapBase<ChurchBulletinItem>
{
    protected override void MapMembers(EntityTypeBuilder<ChurchBulletinItem> entity)
    {
        
    }
}