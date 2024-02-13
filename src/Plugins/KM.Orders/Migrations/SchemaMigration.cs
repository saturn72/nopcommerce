using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace Km.Orders.Migrations
{
    [NopMigration("2023/10/25 09:36:08:9037677", "Nop.Plugin.Misc.KM.Orders schema", MigrationProcessType.Installation)]
    public class SchemaMigration : AutoReversingMigration
    {

        /// <summary>
        /// Collect the UP migration expressions
        /// </summary>
        public override void Up()
        {
            //Create.TableFor<KmUserCustomerMap>();
            //Create.TableFor<KmOrder>();
        }
    }
}
