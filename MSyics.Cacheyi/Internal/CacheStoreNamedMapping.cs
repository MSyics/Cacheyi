/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MSyics.Cacheyi
{
    internal class CacheStoreNamedMapping
    {
        public string CreateKey(string centerName, string storeName)
        {
            return centerName + "@||" + storeName;
        }

        public CacheStore<TKey, TValue> Add<TKey, TValue>(string centerName, string storeName)
        {
            var name = this.CreateKey(centerName, storeName);
            return (CacheStore<TKey, TValue>)this.m_namedMapping.GetOrAdd(name, new CacheStore<TKey, TValue>(name));
        }

        public object Get(string centerName, string storeName)
        {
            object store;
            var name = this.CreateKey(centerName, storeName);
            if (!this.m_namedMapping.TryGetValue(name, out store))
            {
                throw new KeyNotFoundException(name);
            }
            return store;
        }

        private ConcurrentDictionary<string, object> m_namedMapping = new ConcurrentDictionary<string, object>();
    }
}
