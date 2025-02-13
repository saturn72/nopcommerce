using FluentValidation;

namespace KedemMarket.Models.Cart;

public class ShoppingCartItemApiModelValidator : AbstractValidator<ShoppingCartItemApiModel>
{
    public ShoppingCartItemApiModelValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
    }
}
