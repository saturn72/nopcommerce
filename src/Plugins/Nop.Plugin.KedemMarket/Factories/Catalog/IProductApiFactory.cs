using KedemMarket.Models.Cart;
using KedemMarket.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace KedemMarket.Factories.Catalog;
public interface IProductApiFactory
{
    Task<IEnumerable<ProductInfoApiModel>> ToProductInfoApiModelAsync(IEnumerable<Product> products);
    Task<ShoppingCartApiModel> ToShoppingCartApiModelAsync(IEnumerable<ShoppingCartItem> cart);
}
