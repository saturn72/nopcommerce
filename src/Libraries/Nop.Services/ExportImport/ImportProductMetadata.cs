using ClosedXML.Excel;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Services.ExportImport.Help;

namespace Nop.Services.ExportImport;

public partial class ImportProductMetadata
{
    public int EndRow { get; init; }

    public PropertyManager<Product, Language> Manager { get; init; }

    public IList<PropertyByName<Product, Language>> Properties { get; set; }

    public int CountProductsInFile => ProductsInFile.Count;

    public PropertyManager<ExportProductAttribute, Language> ProductAttributeManager { get; init; }

    public PropertyManager<ExportSpecificationAttribute, Language> SpecificationAttributeManager { get; init; }

    public IXLWorksheet DefaultWorksheet { get; set; }

    public List<IXLWorksheet> LocalizedWorksheets { get; set; }

    public int SkuCellNum { get; init; }

    public List<string> AllSku { get; set; }

    public List<int> ProductsInFile { get; set; }
}