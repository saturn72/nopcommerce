﻿namespace KedemMarket.Api.Models.Cart;

public record CheckoutCartApiModel
{
    public IEnumerable<CheckoutCartItemApiModel> Items { get; init; }
}
