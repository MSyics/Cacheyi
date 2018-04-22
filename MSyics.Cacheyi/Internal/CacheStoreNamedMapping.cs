/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MSyics.Cacheyi
{
    internal class CacheStoreNamedMapping
    {
        public string CreateKey(string centerName, string storeName) => $"{centerName}.{storeName}";

        public CacheStore<TKey, TValue> Add<TKey, TValue>(string centerName, string storeName)
        {
            var name = CreateKey(centerName, storeName);
            return (CacheStore<TKey, TValue>)Map.GetOrAdd(name, new CacheStore<TKey, TValue>(name));
        }

        public CacheStore<TUnique, TKey, TValue> Add<TUnique, TKey, TValue>(string centerName, string storeName)
        {
            var name = CreateKey(centerName, storeName);
            return (CacheStore<TUnique, TKey, TValue>)Map.GetOrAdd(name, new CacheStore<TUnique, TKey, TValue>(name));
        }

        public object Get(string centerName, string storeName)
        {
            var name = CreateKey(centerName, storeName);
            if (!Map.TryGetValue(name, out var store)) { throw new KeyNotFoundException(name); }
            return store;
        }

        private ConcurrentDictionary<string, object> Map { get; } = new ConcurrentDictionary<string, object>();
    }
}
