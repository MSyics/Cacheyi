/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Collections.Concurrent;

namespace MSyics.Cacheyi
{
    internal class CacheContext
    {
        public Type CenterType { get; set; }
        public ConcurrentDictionary<Type, Action<CacheCenter>> CenterInitializers => centerInitializers;
        public CacheStoreCollection Stores => stores;

        private static readonly ConcurrentDictionary<Type, Action<CacheCenter>> centerInitializers = new ConcurrentDictionary<Type, Action<CacheCenter>>();
        private static readonly CacheStoreCollection stores = new CacheStoreCollection();
    }
}
