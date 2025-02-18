using FluentMigrator;
using Nop.Data.Extensions;
using Nop.Data.Migrations;

namespace KedemMarket.Migrations;

[NopMigration("2025/02/18 17:08:08:9037678", "KedemMarket schema", MigrationProcessType.Installation)]
public class Migration_20250218_1708 : Migration
{
    protected readonly INopDataProvider _dataProvider;
    private readonly NameCompatibility _nameCompatibility;

    public Migration_20250218_1708(INopDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
        _nameCompatibility = new NameCompatibility();
    }

    /// <summary>
    /// Collect the UP migration expressions
    /// </summary>
    public override void Up()
    {
        CreateTableIfNotExists<KmOrder>();
        var kmOrderTableName = _nameCompatibility.TableNames[typeof(KmOrder)];
        var kmOrderTable = Schema.Table(kmOrderTableName);
        if (kmOrderTable.Column("Data").Exists())
            Delete.Column("Data").FromTable(kmOrderTableName);

        CreateTableIfNotExists<KmUserCustomerMap>();

        CreateTableIfNotExists<NavbarInfo>();
        CreateTableIfNotExists<NavbarElement>();
        CreateTableIfNotExists<NavbarElementVendor>();
    }

    private void CreateTableIfNotExists<TEntity>() where TEntity : BaseEntity
    {
        if (!Schema.Table(_nameCompatibility.TableNames[typeof(TEntity)]).Exists())
            Create.TableFor<TEntity>();

    }
    public override void Down()
    {
    }
}
