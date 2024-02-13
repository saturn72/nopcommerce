
namespace System.Collections.Generic;

public static class EnumerableExtensions
{
    public static bool NotNullAndNotEmpty<T>(this IEnumerable<T> source)
    {
        return source != null && source.Any();
    }
}
