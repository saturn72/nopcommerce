using FluentMigrator.Builders.Create.Table;
using KedemMarket.Infrastructure;
using Nop.Data.Mapping.Builders;

namespace KedemMarket.Migrations.Navbar;

public class NavbarInfoElementBuilder : NopEntityBuilder<NavbarElement>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        var nc = new NameCompatibility();
        table.WithColumn(nameof(NavbarElement.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(NavbarElement.ActiveIcon)).AsString(256)
            .WithColumn(nameof(NavbarElement.Alt)).AsString(2056).Nullable()
            .WithColumn(nameof(NavbarElement.Caption)).AsString(2056)
            .WithColumn(nameof(NavbarElement.Icon)).AsString(256)
            .WithColumn(nameof(NavbarElement.Index)).AsInt32()
            //.WithColumn(nameof(NavbarElement.Tags)).AsString().Nullable()
            .WithColumn(nameof(NavbarElement.Type)).AsString(1024).NotNullable()
            //.WithColumn(nameof(NavbarElement.Value)).AsString()
            .WithColumn(nameof(NavbarElement.NavbarInfoId)).AsInt32().ForeignKey(nc.TableNames[typeof(NavbarInfo)], nameof(NavbarInfo.Id)).NotNullable();
    }
}
