namespace KM.Api.Models.Checkout;

public class CartTransactionApiModelValidator : AbstractValidator<CartTransactionApiModel>
{
    public CartTransactionApiModelValidator()
    {
        RuleFor(x => x.Items).Must(x => x.NotNullAndNotNotEmpty());

        RuleFor(x => x.PaymentMethod).Must(x => x.HasValue());

        RuleFor(x => x.Status).Must(x => x.IsPaidStatus());

        RuleFor(x => x.StoreId).GreaterThan(0);
        
        RuleFor(x => x.UserId).Must(x => x.HasValue());
    }
}
