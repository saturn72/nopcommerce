using Nop.Data.Mapping;

namespace KM.Catalog.Mapping;

public partial class NameCompatibility : INameCompatibility
{
    public Dictionary<Type, string> TableNames => new Dictionary<Type, string>();

    public Dictionary<(Type, string), string> ColumnName => new Dictionary<(Type, string), string>();
}