
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Km.Catalog.Migrations;

public class StoreSnapshotBuilder : NopEntityBuilder<StoreSnapshot>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(StoreSnapshot.CreatedOnUtc)).AsDateTime2().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(StoreSnapshot.Version)).AsInt32()
            .WithColumn(nameof(StoreSnapshot.Json)).AsString().NotNullable();
    }
}
