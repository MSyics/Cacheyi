using System;

namespace MSyics.Cacheyi
{
    internal interface ICacheValueBuilder<TKeyed, TKey, TValue>
    {
        TValue GetValue(TKeyed keyed, TKey key);
    }

    internal sealed class FuncCacheValueBuilder<TKeyed, TKey, TValue> : ICacheValueBuilder<TKeyed, TKey, TValue>
    {
        public Func<TKeyed, TKey, TValue> Build { get; set; }
        public TValue GetValue(TKeyed keyed, TKey key) => Build(keyed, key);
    }
}
