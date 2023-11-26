
using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace Km.Catalog.Migrations;

public class KmStoresSnapshotBuilder : NopEntityBuilder<KmStoresSnapshot>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(KmStoresSnapshot.CreatedOnUtc)).AsDateTime2().Nullable().WithDefault(SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(KmStoresSnapshot.Version)).AsInt32()
            .WithColumn(nameof(KmStoresSnapshot.Json)).AsString().NotNullable();
    }
}
