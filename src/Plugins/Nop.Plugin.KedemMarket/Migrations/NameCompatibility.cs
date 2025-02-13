using Nop.Data.Mapping;

namespace KedemMarket.Migrations;

public partial class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new();

    public Dictionary<(Type, string), string> ColumnName => new();
}