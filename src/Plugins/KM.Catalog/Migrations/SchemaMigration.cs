using FluentMigrator;
using Nop.Data.Migrations;

namespace Km.Catalog.Migrations;

[NopMigration("2023/06/27 09:36:08:9037677", "Km.Catalog schema", MigrationProcessType.Installation)]
public class SchemaMigration : AutoReversingMigration
{
    public override void Up()
    {
        // Create.TableFor<Storesnapshot>();
    }
}
