﻿using FluentMigrator;
using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;

namespace KedemMarket.Migrations.Domain;

public class KmOrderBuilder : NopEntityBuilder<KmOrder>
{
    public override void MapEntity(CreateTableExpressionBuilder table)
    {

        table
                .WithColumn(nameof(KmOrder.CreatedOnUtc))
                    .AsDateTime2()
                    .Nullable()
                    .WithDefault(SystemMethods.CurrentUTCDateTime)

                .WithColumn(nameof(KmOrder.Status))
                    .AsString(128).NotNullable()

                .WithColumn(nameof(KmOrder.KmUserId))
                    .AsString(256).NotNullable()

            .WithColumn(nameof(KmOrder.NopOrderId))
                .AsInt32()
                .NotNullable()

            .WithColumn(nameof(KmOrder.Errors))
                .AsString()
                .NotNullable();
    }
}
