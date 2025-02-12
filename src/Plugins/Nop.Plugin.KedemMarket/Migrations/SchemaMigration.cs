using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace KedemMarket.Navbar.Migrations;

[NopMigration("2024/12/27 10:45:08:9037677", "KM.Navbar schema", MigrationProcessType.NoMatter)]
public class SchemaMigration : ForwardOnlyMigration
{

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<NavbarInfo>();
        Create.TableFor<NavbarElement>();
        Create.TableFor<NavbarElementVendor>();
    }
}
