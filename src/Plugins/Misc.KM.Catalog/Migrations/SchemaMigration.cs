using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.KM.Catalog.Domain;

namespace Nop.Plugin.Misc.KM.Catalog.Migrations
{
    [NopMigration("2023/06/27 09:36:08:9037677", "Nop.Plugin.Misc.KM.Catalog schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {
        public override void Up()
        {
            Create.TableFor<MediaItemInfo>();
        }
    }
}
