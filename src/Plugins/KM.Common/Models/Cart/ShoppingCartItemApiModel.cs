﻿using KedemMarket.Common.Models.Catalog;

namespace KedemMarket.Common.Models.Cart;
public record ShoppingCartItemApiModel
{
    public ProductInfoApiModel? ProductInfo { get; init; }
    public int Quantity { get; init; }
    public int ProductId { get; init; }
    public int VariantId { get; init; } //product-attribute-Id
    public int OptionId { get; init; } // product-attribute-value-id
}
