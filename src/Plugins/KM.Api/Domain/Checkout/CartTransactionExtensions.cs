namespace KM.Api.Domain.Checkout;

public static class CartTransactionExtensions
{
    public static bool IsPaidStatus(this string status) => IsStatus(status, CartTransactionStatus.Paid);

    private static bool IsStatus(string status, string expected) =>
        status.Equals(expected, StringComparison.OrdinalIgnoreCase);
}
