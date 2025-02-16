using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace KedemMarket.Migrations.Navbar;
public class NavbarInfoBuilder : NopEntityBuilder<NavbarInfo>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NavbarInfo.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(NavbarInfo.CreatedOnUtc)).AsDateTime2().WithDefault(FluentMigrator.SystemMethods.CurrentUTCDateTime)
            .WithColumn(nameof(NavbarInfo.Deleted)).AsBoolean().WithDefaultValue(false)
            .WithColumn(nameof(NavbarInfo.DisplayOrder)).AsInt32()
            .WithColumn(nameof(NavbarInfo.Description)).AsString(1024).Nullable()
            .WithColumn(nameof(NavbarInfo.LimitedToStores)).AsBoolean()
            .WithColumn(nameof(NavbarInfo.Name)).AsString(64).Nullable()
            .WithColumn(nameof(NavbarInfo.Published)).AsBoolean()
            .WithColumn(nameof(NavbarInfo.UpdatedOnUtc)).AsDateTime2().Nullable();
    }
}
