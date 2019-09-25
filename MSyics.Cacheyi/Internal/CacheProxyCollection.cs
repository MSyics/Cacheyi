using System.Collections.ObjectModel;

namespace MSyics.Cacheyi
{
    internal sealed class CacheProxyCollection<TKey, TValue> : KeyedCollection<TKey, CacheProxy<TKey, TValue>>
    {
        public CacheProxyCollection() : base(null, 0) { }
        protected override TKey GetKeyForItem(CacheProxy<TKey, TValue> item) => item.Key;
    }
}
