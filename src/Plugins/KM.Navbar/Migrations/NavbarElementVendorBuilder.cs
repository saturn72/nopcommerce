using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace KM.Navbar.Migrations;

public class NavbarElementVendorBuilder : NopEntityBuilder<NavbarElementVendor>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {
        table
            .WithColumn(nameof(NavbarElementVendor.Id)).AsInt32().PrimaryKey().Identity()
            .WithColumn(nameof(NavbarElementVendor.DisplayOrder)).AsInt32()
            .WithColumn(nameof(NavbarElementVendor.IsFeaturedVendor)).AsBoolean().WithDefaultValue(false)
            .WithColumn(nameof(NavbarElementVendor.NavbarElementId)).AsInt32()
            .WithColumn(nameof(NavbarElementVendor.Published)).AsBoolean()
            .WithColumn(nameof(NavbarElementVendor.VendorId)).AsInt32();
    }
}