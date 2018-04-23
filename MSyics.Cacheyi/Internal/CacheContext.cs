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

        public ConcurrentDictionary<Type, Action<CacheCenter>> CenterInitializerTypedMapping => _centerInitializerTypedMapping;
        private static readonly ConcurrentDictionary<Type, Action<CacheCenter>> _centerInitializerTypedMapping = new ConcurrentDictionary<Type, Action<CacheCenter>>();

        public CacheStoreNamedMapping StoreInstanceNamedMapping => _storeInstanceNamedMapping;
        private static readonly CacheStoreNamedMapping _storeInstanceNamedMapping = new CacheStoreNamedMapping();
    }
}
