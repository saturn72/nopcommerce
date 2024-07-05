
namespace KM.Api.Services.Checkout;

public static class OrderStringExtensions
{
    public static string ToSystemPaymentMethod(this string paymentMethod)
    {
        return paymentMethod.ToLower() switch
        {
            "cash" => "Payments.CashOnDelivery",
            "cash-on-delivery" => "Payments.CashOnDelivery",
            _ => paymentMethod,
        };
    }
}