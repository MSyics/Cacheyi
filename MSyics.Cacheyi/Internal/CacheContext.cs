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
        public ConcurrentDictionary<Type, Action<CacheCenter>> CenterInitializers => Context.CenterInitializers;
        public CacheStoreCollection Stores => Context.Stores;

        static class Context
        {
            public static readonly ConcurrentDictionary<Type, Action<CacheCenter>> CenterInitializers = new ConcurrentDictionary<Type, Action<CacheCenter>>();
            public static readonly CacheStoreCollection Stores = new CacheStoreCollection();
        }
    }
}
