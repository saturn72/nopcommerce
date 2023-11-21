
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Km.Catalog.Migrations;

public class MediaItemInfoBuilder : NopEntityBuilder<KmMediaItemInfo>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(KmMediaItemInfo.CreatedOnUtc)).AsDateTime2().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(KmMediaItemInfo.EntityId)).AsInt32()
            .WithColumn(nameof(KmMediaItemInfo.EntityType)).AsString(256)
            .WithColumn(nameof(KmMediaItemInfo.Storage)).AsString(64).Nullable()
            .WithColumn(nameof(KmMediaItemInfo.StorageIdentifier)).AsString(256).Nullable();
    }
}
