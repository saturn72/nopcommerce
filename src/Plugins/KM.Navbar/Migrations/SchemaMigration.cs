using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace KM.Navbar.Migrations;

[NopMigration("2024/12/14 09:40:08:9037677", "Nop.Plugin.Misc.KM.Navbar schema", MigrationProcessType.Installation)]
public class SchemaMigration : ForwardOnlyMigration
{

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<NavbarInfo>();
        Create.TableFor<NavbarElement>();
    }
}
