using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial class ImportOrderMetadata
{
    public int EndRow { get; init; }

    public PropertyManager<Order, Language> Manager { get; init; }

    public IList<PropertyByName<Order, Language>> Properties { get; set; }

    public int CountOrdersInFile { get; set; }

    public PropertyManager<OrderItem, Language> OrderItemManager { get; init; }

    public List<Guid> AllOrderGuids { get; set; }

    public List<Guid> AllCustomerGuids { get; set; }
}