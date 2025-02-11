using KedemMarket.Common.Models.Cart;
using KedemMarket.Common.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Orders;

namespace KedemMarket.Common.Factories;
public interface IProductApiFactory
{
    Task<IEnumerable<ProductInfoApiModel>> ToProductInfoApiModelAsync(IEnumerable<Product> products);
    Task<ShoppingCartApiModel> ToShoppingCartApiModelAsync(IEnumerable<ShoppingCartItem> cart);
}
