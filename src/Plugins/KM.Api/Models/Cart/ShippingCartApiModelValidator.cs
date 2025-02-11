using KedemMarket.Common.Models.Cart;

namespace KedemMarket.Api.Models.Cart;

public class CartApiModelValidator : AbstractValidator<ShoppingCartApiModel>
{
    public CartApiModelValidator()
    {
        RuleFor(x => x.Items).Must(x => x.NotNullAndNotNotEmpty());
    }
}
