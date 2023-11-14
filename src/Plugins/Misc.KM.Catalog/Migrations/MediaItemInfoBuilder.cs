
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.KM.Catalog.Domain;

namespace Nop.Plugin.Misc.KM.Catalog.Migrations
{
    public class MediaItemInfoBuilder : NopEntityBuilder<MediaItemInfo>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(MediaItemInfo.Id)).AsInt32().PrimaryKey().Identity()
                .WithColumn(nameof(MediaItemInfo.CreatedOnUtc)).AsDateTime2().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn(nameof(MediaItemInfo.EntityId)).AsInt32()
                .WithColumn(nameof(MediaItemInfo.EntityType)).AsString(256)
                .WithColumn(nameof(MediaItemInfo.Storage)).AsString(64).Nullable()
                .WithColumn(nameof(MediaItemInfo.StorageIdentifier)).AsString(256).Nullable();
        }
    }
}
