

namespace KM.Api.Factories;
public interface ICheckoutCartApiFactory
{
    Task<CheckoutCartApiModel> PrepareCheckoutCartApiModelAsync(IList<ShoppingCartItem> cart);
}
