using System.Collections.ObjectModel;

namespace Msyics.Cacheyi.Tests;

internal static class Extensions
{
    public static void AddRange<T>(this ObservableCollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Add(item);
        }
    }

    public static IEnumerable<TestValue> ToTestValue(this IEnumerable<int> source)
    {
        foreach (var item in source)
        {
            yield return new(item);
        }
    }
}
