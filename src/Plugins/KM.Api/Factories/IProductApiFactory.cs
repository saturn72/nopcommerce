using KM.Api.Models.Catalog;
using Nop.Core.Domain.Catalog;

namespace KM.Api.Factories;
public interface IProductApiFactory
{
    Task<IEnumerable<ProductInfoApiModel>> ToProductInfoApiModel(IEnumerable<Product> products);
    Task<ShoppingCartApiModel> ToShoppingCartApiModel(IEnumerable<ShoppingCartItem> cart);
}
