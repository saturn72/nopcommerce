using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Extensions;
using Nop.Data.Mapping.Builders;

namespace Km.Api.Migrations
{
    public class KmUserCustomerMapInfoBuilder : NopEntityBuilder<KmUserCustomerMap>
    {
        public override void MapEntity(CreateTableExpressionBuilder table)
        {
            table
                .WithColumn(nameof(KmUserCustomerMap.CustomerId)).AsInt32().ForeignKey<Customer>()
                .WithColumn(nameof(KmUserCustomerMap.KmUserId)).AsString(256).NotNullable()
                .WithColumn(nameof(KmUserCustomerMap.CreatedOnUtc)).AsDateTime2().WithDefault(SystemMethods.CurrentUTCDateTime)
                .WithColumn(nameof(KmUserCustomerMap.ProviderId)).AsString(256).Nullable()
                .WithColumn(nameof(KmUserCustomerMap.TenantId)).AsString(256).Nullable();
        }
    }
}
