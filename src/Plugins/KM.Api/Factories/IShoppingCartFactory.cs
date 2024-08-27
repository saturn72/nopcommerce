
namespace KM.Api.Factories;
public interface IShoppingCartFactory
{
    Task<CreateOrderRequest> ToCreateOrderRequest(CartTransactionApiModel model, List<string> errors);
    Task<IList<ShoppingCartItem>> ToShoppingCartItems(IEnumerable<ShoppingCartItemApiModel> items, List<string> errors);
}
