﻿using KedemMarket.Common.Models.Catalog;

namespace KedemMarket.Api.Models.Cart;

public record CheckoutCartItemApiModel : ShoppingCartModel.ShoppingCartItemModel
{
    public ProductInfoApiModel ProductInfo { get; init; }
    public int VariantId { get; init; } //product-attribute-Id
    public int OptionId { get; init; } // product-attribute-value-id
    public string Thumbnail { get; init; } // product-attribute-value-id

}
