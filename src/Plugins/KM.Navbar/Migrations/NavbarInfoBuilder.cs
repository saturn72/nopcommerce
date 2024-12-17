using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace KM.Navbar.Migrations;
public class NavbarInfoBuilder : NopEntityBuilder<NavbarInfo>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NavbarInfo.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(NavbarInfo.DisplayOrder)).AsInt32()
            .WithColumn(nameof(NavbarInfo.Published)).AsBoolean()
            .WithColumn(nameof(NavbarInfo.Name)).AsString(256).Nullable();

    }
}
