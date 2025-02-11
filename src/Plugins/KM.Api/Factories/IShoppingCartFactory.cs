﻿
using KedemMarket.Common.Models.Cart;

namespace KedemMarket.Api.Factories;
public interface IShoppingCartFactory
{
    Task<CreateOrderRequest> ToCreateOrderRequest(CartTransactionApiModel model, List<string> errors);
    Task<IList<ShoppingCartItem>> ToShoppingCartItems(IEnumerable<ShoppingCartItemApiModel> items, List<string> errors);
    Task<CheckoutCartApiModel> PrepareCheckoutCartApiModelAsync(IList<ShoppingCartItem> cart);
}
