using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace KM.Navbar.Migrations;

public class NavbarInfoElementBuilder : NopEntityBuilder<NavbarElement>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NavbarElement.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(NavbarElement.Alt)).AsString(1024).Nullable()
            .WithColumn(nameof(NavbarElement.Icon)).AsString(128)
            .WithColumn(nameof(NavbarElement.Index)).AsString(400)
            .WithColumn(nameof(NavbarElement.Caption)).AsString()
            .WithColumn(nameof(NavbarElement.Tags)).AsString().Nullable()
            .WithColumn(nameof(NavbarElement.Type)).AsString(256).NotNullable()
            .WithColumn(nameof(NavbarElement.Value)).AsString()
            .WithColumn(nameof(NavbarElement.NavbarInfoId)).AsInt32().ForeignKey($"{nameof(NavbarInfo)}s", nameof(NavbarElement.NavbarInfoId)).NotNullable();
    }
}
