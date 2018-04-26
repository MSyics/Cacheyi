/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MSyics.Cacheyi
{
    internal sealed class CacheProxyCollection<TKey, TValue> : KeyedCollection<TKey, CacheProxy<TKey, TValue>>
    {
        public CacheProxyCollection() : base(null, 0) { }
        protected override TKey GetKeyForItem(CacheProxy<TKey, TValue> item) => item.Key;
    }
}
