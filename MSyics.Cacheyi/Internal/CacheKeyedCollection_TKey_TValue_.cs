/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュ関連オブジェクトを格納するクラスです。
    /// </summary>
    /// <typeparam name="TKey">キー</typeparam>
    /// <typeparam name="TValue">キャッシュ</typeparam>
    internal sealed class CacheKeyedCollection<TKey, TValue> : KeyedCollection<TKey, CacheProxy<TKey, TValue>>
    {
        public CacheKeyedCollection() : base(null, 0) { }
        protected override TKey GetKeyForItem(CacheProxy<TKey, TValue> item) => item.CacheKey;
    }

    /// <summary>
    /// キャッシュ関連オブジェクトを格納するクラスです。
    /// </summary>
    /// <typeparam name="TKey">キー</typeparam>
    /// <typeparam name="TValue">キャッシュ</typeparam>
    internal sealed class CacheKeyedCollection<TUnique, TKey, TValue> : KeyedCollection<TUnique, CacheProxy<TUnique, TKey, TValue>>
    {
        public CacheKeyedCollection() : base(null, 0) { }
        protected override TUnique GetKeyForItem(CacheProxy<TUnique, TKey, TValue> item) => item.CacheKey.UniqueKey;
    }
}
