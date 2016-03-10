/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュ関連オブジェクトおよびオブジェクトを区別するキーを派生します。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    internal interface ICacheKeyBuilder<TKey>
    {
        /// <summary>
        /// キャッシュのキーを取得します。
        /// </summary>
        /// <param name="key">オブジェクトを区別するキー</param>
        CacheKey<TKey> GetCacheKey(TKey key);
    }

    /// <summary>
    /// デリゲートで CacheKey ビルダー を構築できるようにします。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    internal sealed class FuncCacheKeyBuilder<TKey> : ICacheKeyBuilder<TKey>
    {
        public Func<TKey, int> Builder { get; set; }

        public CacheKey<TKey> GetCacheKey(TKey key)
        {
            #region Doer
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            #endregion

            return new CacheKey<TKey>(BuidUniqueKey(key), key);
        }

        private int BuidUniqueKey(TKey key)
        {
            #region Doer
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            #endregion

            return Builder(key);
        }
    }
}
