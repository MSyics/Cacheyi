using System.Collections.ObjectModel;

namespace MSyics.Cacheyi;

internal sealed class CacheProxyCollection<TKey, TItem> : KeyedCollection<TKey, TItem>
    where TItem : IKeyed<TKey>
{
    public CacheProxyCollection() : base(null, 0) { }

    protected override TKey GetKeyForItem(TItem item) => item.Key;

#if NETSTANDARD2_0
    public bool TryGetValue(TKey key, out TItem item)
    {
        if (Contains(key))
        {
            item = this[key];
            return true;
        }
        else
        {
            item = default;
            return false;
        }
    }
#endif
}
