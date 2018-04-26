/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi
{
    internal interface ICacheKeyBuilder<TKeyed, TKey>
    {
        TKey GetKey(TKeyed keyed);
    }

    internal sealed class FuncCacheKeyFactory<TKeyed, TKey> : ICacheKeyBuilder<TKeyed, TKey>
    {
        public Func<TKeyed, TKey> Build { get; set; }
        public TKey GetKey(TKeyed keyed) => Build(keyed);
    }
}
