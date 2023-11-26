namespace KM.Orders.Models.Cart;

public class ShoppingCartApiModelValidator : AbstractValidator<ShoppingCartApiModel>
{
    public ShoppingCartApiModelValidator()
    {
        RuleFor(x => x.StoreId).GreaterThan(0);

        RuleFor(x => x.UserId).Must(x => x.HasValue());

        RuleFor(x => x.Items).Must(x => x.NotNullAndNotNotEmpty());
    }
}
