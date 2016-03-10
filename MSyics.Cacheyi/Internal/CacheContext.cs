/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Collections.Concurrent;

namespace MSyics.Cacheyi
{
    internal class CacheContext
    {
        private static readonly ConcurrentDictionary<Type, Action<CacheCenter>> m_centerInitializerTypedMapping = new ConcurrentDictionary<Type, Action<CacheCenter>>();
        private static readonly CacheStoreNamedMapping m_storeInstanceNamedMapping = new CacheStoreNamedMapping();

        public Type CenterType { get; set; }
        public ConcurrentDictionary<Type, Action<CacheCenter>> CenterInitializerTypedMapping => m_centerInitializerTypedMapping;
        public CacheStoreNamedMapping StoreInstanceNamedMapping => m_storeInstanceNamedMapping;
    }
}
