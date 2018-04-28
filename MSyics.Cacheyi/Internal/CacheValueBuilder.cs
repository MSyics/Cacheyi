/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi
{
    internal interface ICacheValueBuilder<TKey, TValue>
    {
        TValue GetValue(TKey key);
    }

    internal sealed class FuncCacheValueBuilder<TKey, TValue> : ICacheValueBuilder<TKey, TValue>
    {
        public Func<TKey, TValue> Build { get; set; }
        public TValue GetValue(TKey key) => Build(key);
    }
}
