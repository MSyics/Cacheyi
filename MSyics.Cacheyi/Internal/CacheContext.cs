using System;
using System.Collections.Concurrent;

namespace MSyics.Cacheyi
{
    internal class CacheContext
    {
        public ConcurrentDictionary<Type, Action<CacheCenter>> CenterInitializers => centerInitializers;
        public CacheStoreCollection Stores => stores;

        private static readonly ConcurrentDictionary<Type, Action<CacheCenter>> centerInitializers = new ConcurrentDictionary<Type, Action<CacheCenter>>();
        private static readonly CacheStoreCollection stores = new CacheStoreCollection();
    }
}
