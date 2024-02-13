
using System.Globalization;

namespace System;

public static class DateTimeExtensions
{
    public static string ToIso8601(this DateTime? dateTime)
    {
        return dateTime.HasValue?
            dateTime.Value.ToString("o", CultureInfo.InvariantCulture):
            null;
    }
}
