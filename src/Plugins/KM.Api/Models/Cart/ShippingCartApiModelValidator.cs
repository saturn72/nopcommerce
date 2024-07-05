namespace KM.Api.Models.Cart;

public class CartApiModelValidator : AbstractValidator<ShoppingCartApiModel>
{
    public CartApiModelValidator()
    {
        RuleFor(x => x.StoreId).GreaterThan(0);

        RuleFor(x => x.UserId).Must(x => x.HasValue());

        RuleFor(x => x.Items).Must(x => x.NotNullAndNotNotEmpty());
    }
}
