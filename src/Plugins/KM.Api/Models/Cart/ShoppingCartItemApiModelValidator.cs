namespace KM.Api.Models.Cart;

public class ShoppingCartItemApiModelValidator : AbstractValidator<ShoppingCartItemApiModel>
{
    public ShoppingCartItemApiModelValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
    }
}
