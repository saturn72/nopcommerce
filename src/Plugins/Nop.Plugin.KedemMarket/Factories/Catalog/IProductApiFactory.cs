namespace KedemMarket.Factories.Catalog;
public interface IProductApiFactory
{
    Task<IEnumerable<ProductInfoApiModel>> ToProductInfoApiModelAsync(IEnumerable<Product> products);
    Task<ShoppingCartApiModel> ToShoppingCartApiModelAsync(IEnumerable<ShoppingCartItem> cart);
}
