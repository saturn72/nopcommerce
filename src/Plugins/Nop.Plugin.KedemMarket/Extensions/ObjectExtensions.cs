namespace System;

public static class ObjectExtensions
{
    public static void ThrowArgumentNullException<T>(this T source, string? message = null) where T : class
    {
        if (source == default)
            throw new ArgumentNullException(message ?? nameof(source));
    }

    public static void ThrowArgumentException<T>(
        this T source,
        Func<T, bool> isValid,
    string? message = null,
    string? paramName = null
    ) where T : class
    {
        if (!isValid(source))
            throw new ArgumentException(message, paramName);
    }
}