using FluentMigrator.Builders.Create.Table;
using KedemMarket.Admin.Domain;
using KedemMarket.Infrastructure;
using Nop.Data.Mapping.Builders;

namespace KedemMarket.Migrations.Navbar;

public class NavbarElementVendorBuilder : NopEntityBuilder<NavbarElementVendor>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        var nc = new NameCompatibility();
        table
            .WithColumn(nameof(NavbarElementVendor.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(NavbarElementVendor.DisplayOrder)).AsInt32()
            .WithColumn(nameof(NavbarElementVendor.IsFeaturedVendor)).AsBoolean().WithDefaultValue(false)
            .WithColumn(nameof(NavbarElementVendor.NavbarElementId))
                .AsInt32()
                .ForeignKey(nc.TableNames[typeof(NavbarElement)], nameof(NavbarElement.Id)).NotNullable()
            .WithColumn(nameof(NavbarElementVendor.Published)).AsBoolean()
            .WithColumn(nameof(NavbarElementVendor.VendorId)).AsInt32();
    }
}