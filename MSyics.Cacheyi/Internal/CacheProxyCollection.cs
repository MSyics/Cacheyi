using System.Collections.ObjectModel;

namespace MSyics.Cacheyi;

internal sealed class CacheProxyCollection<TKey, TValue> : KeyedCollection<TKey, CacheProxy<TKey, TValue>>
{
    public CacheProxyCollection() : base(null, 0) { }

    protected override TKey GetKeyForItem(CacheProxy<TKey, TValue> item) => item.Key;

#if NETSTANDARD2_0
    public bool TryGetValue(TKey key, out CacheProxy<TKey, TValue> item)
    {
        if (Contains(key))
        {
            item = this[key];
            return true;
        }
        else
        {
            item = null;
            return false;
        }
    }
#endif
}
