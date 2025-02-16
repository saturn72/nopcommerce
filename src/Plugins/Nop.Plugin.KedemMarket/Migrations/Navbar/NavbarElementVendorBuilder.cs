using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;

namespace KedemMarket.Migrations.Navbar;

public class NavbarElementVendorBuilder : NopEntityBuilder<NavbarElementVendor>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NavbarElementVendor.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(NavbarElementVendor.DisplayOrder)).AsInt32()
            .WithColumn(nameof(NavbarElementVendor.IsFeaturedVendor)).AsBoolean().WithDefaultValue(false)
            .WithColumn(nameof(NavbarElementVendor.NavbarElementId)).AsInt32().ForeignKey<NavbarElement>()
            .WithColumn(nameof(NavbarElementVendor.Published)).AsBoolean()
            .WithColumn(nameof(NavbarElementVendor.VendorId)).AsInt32();
    }
}