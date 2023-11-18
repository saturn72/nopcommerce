using FluentMigrator.Builders.Create.Table;
using Nop.Data.Mapping.Builders;
using Nop.Plugin.Misc.KM.Catalog.Domain;

namespace Nop.Plugin.Misc.KM.Catalog.Mapping.Builders
{
    public class PluginBuilder : NopEntityBuilder<KmMediaItemInfo>
    {
        #region Methods

        public override void MapEntity(CreateTableExpressionBuilder table)
        {
        }

        #endregion
    }
}