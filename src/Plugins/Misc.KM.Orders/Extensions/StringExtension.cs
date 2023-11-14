namespace System;

public static class StringExtensions
{
    public static bool HasValue(this string value)
    {
        return !string.IsNullOrEmpty(value) && value.Trim().Any();
    }
    public static bool HasNoValue(this string value)
    {
        return string.IsNullOrEmpty(value) || !value.Trim().Any();
    }
}