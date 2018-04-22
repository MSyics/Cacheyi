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
        private static readonly ConcurrentDictionary<Type, Action<CacheCenter>> _centerInitializerTypedMapping = new ConcurrentDictionary<Type, Action<CacheCenter>>();
        private static readonly CacheStoreNamedMapping _storeInstanceNamedMapping = new CacheStoreNamedMapping();

        public Type CenterType { get; set; }
        public ConcurrentDictionary<Type, Action<CacheCenter>> CenterInitializerTypedMapping => _centerInitializerTypedMapping;
        public CacheStoreNamedMapping StoreInstanceNamedMapping => _storeInstanceNamedMapping;
    }
}
