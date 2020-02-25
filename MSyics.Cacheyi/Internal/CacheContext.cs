using System;
using System.Collections.Concurrent;

namespace MSyics.Cacheyi
{
    internal class CacheContext
    {
        public ConcurrentDictionary<Type, Action<object>> CenterInitializers => centerInitializers;
        public CacheStoreCollection Stores => stores;

        private static readonly ConcurrentDictionary<Type, Action<object>> centerInitializers = new ConcurrentDictionary<Type, Action<object>>();
        private static readonly CacheStoreCollection stores = new CacheStoreCollection();
    }
}
