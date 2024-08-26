using Nop.Web.Framework.Validators;

namespace KM.Api.Models.Checkout;

public class CartTransactionApiModelValidator : AbstractValidator<CartTransactionApiModel>
{
    public CartTransactionApiModelValidator(CustomerSettings customerSettings)
    {
        RuleFor(x => x.Items).Must(x => x.NotNullAndNotNotEmpty());

        RuleFor(x => x.PaymentMethod).Must(x => x.HasValue());

        RuleFor(x => x.Status).Must(x => x.IsPaidStatus());

        RuleFor(x => x.BillingInfo.Email).IsEmailAddress();
        RuleFor(x => x.BillingInfo.Fullname).Must(x => x.HasValue());
        RuleFor(x => x.BillingInfo.Phone).IsPhoneNumber(customerSettings);
        RuleFor(x => x.BillingInfo.Address.City).Must(x => x.HasValue());
        RuleFor(x => x.BillingInfo.Address.Street).Must(x => x.HasValue());

        RuleFor(x => x.ShippingInfo.Email).IsEmailAddress();
        RuleFor(x => x.ShippingInfo.Fullname).Must(x => x.HasValue());
        RuleFor(x => x.ShippingInfo.Phone).IsPhoneNumber(customerSettings);
        RuleFor(x => x.ShippingInfo.Address.City).Must(x => x.HasValue());
        RuleFor(x => x.ShippingInfo.Address.Street).Must(x => x.HasValue());
    }
}
