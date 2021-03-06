﻿using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MSyics.Cacheyi
{
    internal class CacheStoreCollection
    {
        private readonly ConcurrentDictionary<string, object> stores = new ConcurrentDictionary<string, object>();

        public CacheStore<TKey, TValue> Add<TKey, TValue>(string name) => (CacheStore<TKey, TValue>)stores.GetOrAdd(name, new CacheStore<TKey, TValue>());
        public CacheStore<TKeyed, TKey, TValue> Add<TKeyed, TKey, TValue>(string name) => (CacheStore<TKeyed, TKey, TValue>)stores.GetOrAdd(name, new CacheStore<TKeyed, TKey, TValue>());

        public object GetValue(string name)
        {
            if (!stores.TryGetValue(name, out var store)) { throw new KeyNotFoundException(name); }
            return store;
        }

        public CacheStore<TKey, TValue> GetValue<TKey, TValue>(string name)
        {
            return (CacheStore<TKey, TValue>)GetValue(name);
        }

        public CacheStore<TKeyed, TKey, TValue> GetValue<TKeyed, TKey, TValue>(string name)
        {
            return (CacheStore<TKeyed, TKey, TValue>)GetValue(name);
        }
    }
}
