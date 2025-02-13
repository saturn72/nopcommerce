using FluentValidation;

namespace KedemMarket.Models.Cart;

public class CartApiModelValidator : AbstractValidator<ShoppingCartApiModel>
{
    public CartApiModelValidator()
    {
        RuleFor(x => x.Items).Must(x => x.NotNullAndNotNotEmpty());
    }
}
