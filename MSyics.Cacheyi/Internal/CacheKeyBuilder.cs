/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Diagnostics.Contracts;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュ関連オブジェクトおよびオブジェクトを区別するキーを派生します。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    internal interface ICacheKeyBuilder<TUnique, TKey>
    {
        /// <summary>
        /// キャッシュのキーを取得します。
        /// </summary>
        /// <param name="key">オブジェクトを区別するキー</param>
        CacheKey<TUnique, TKey> GetCacheKey(TKey key);
    }

    /// <summary>
    /// デリゲートで CacheKey ビルダー を構築できるようにします。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    internal sealed class FuncCacheKeyBuilder<TUnique, TKey> : ICacheKeyBuilder<TUnique, TKey>
    {
        public Func<TKey, TUnique> Builder { get; set; }

        public CacheKey<TUnique, TKey> GetCacheKey(TKey key)
        {
            #region Doer
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            #endregion

            return new CacheKey<TUnique, TKey>(BuidUniqueKey(key), key);
        }

        private TUnique BuidUniqueKey(TKey key)
        {
            #region Doer
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            #endregion

            return Builder(key);
        }
    }
}
