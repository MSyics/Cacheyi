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
        private ConcurrentDictionary<string, object> Stores = new ConcurrentDictionary<string, object>();

        public CacheStore<TKey, TValue> Add<TKey, TValue>(string name) => (CacheStore<TKey, TValue>)Stores.GetOrAdd(name, new CacheStore<TKey, TValue>());
        public CacheStore<TKeyed, TKey, TValue> Add<TKeyed, TKey, TValue>(string name) => (CacheStore<TKeyed, TKey, TValue>)Stores.GetOrAdd(name, new CacheStore<TKeyed, TKey, TValue>());

        public object GetValue(string name)
        {
            if (!Stores.TryGetValue(name, out var store)) { throw new KeyNotFoundException(name); }
            return store;
        }
    }
}
