/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MSyics.Cacheyi
{
    internal class CacheStoreCollection
    {
        private ConcurrentDictionary<string, object> Stores { get; } = new ConcurrentDictionary<string, object>();

        private string GetKey(string centerName, string storeName) => $"{centerName}.{storeName}";

        public CacheStore<TKey, TValue> Add<TKey, TValue>(string centerName, string storeName) =>
            (CacheStore<TKey, TValue>)Stores.GetOrAdd(GetKey(centerName, storeName), new CacheStore<TKey, TValue>());

        public CacheStore<TKeyed, TKey, TValue> Add<TKeyed, TKey, TValue>(string centerName, string storeName) =>
            (CacheStore<TKeyed, TKey, TValue>)Stores.GetOrAdd(GetKey(centerName, storeName), new CacheStore<TKeyed, TKey, TValue>());

        public object Get(string centerName, string storeName)
        {
            var name = GetKey(centerName, storeName);
            if (!Stores.TryGetValue(name, out var store)) { throw new KeyNotFoundException(name); }
            return store;
        }
    }
}
