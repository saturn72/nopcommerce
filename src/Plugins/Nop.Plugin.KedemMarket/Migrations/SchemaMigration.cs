using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace KedemMarket.Migrations;

[NopMigration("2025/02/15 10:45:08:9037678", "KedemMarket schema", MigrationProcessType.Installation)]
public class SchemaMigration : ForwardOnlyMigration
{

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        Create.TableFor<KmOrder>();
        Create.TableFor<KmUserCustomerMap>();

        Create.TableFor<NavbarInfo>();
        Create.TableFor<NavbarElement>();
        Create.TableFor<NavbarElementVendor>();
    }
}
