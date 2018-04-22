/****************************************************************
© 2018 MSyics
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
    /// <typeparam name="TKeyed"></typeparam>
    internal interface ICacheKeyBuilder<TKey, TKeyed>
    {
        /// <summary>
        /// キャッシュのキーを取得します。
        /// </summary>
        /// <param name="keyed">オブジェクトを区別するキー</param>
        TKey GetKey(TKeyed keyed);
    }

    /// <summary>
    /// デリゲートで CacheKey ビルダー を構築できるようにします。
    /// </summary>
    /// <typeparam name="TKeyed"></typeparam>
    internal sealed class FuncCacheKeyBuilder<TKey, TKeyed> : ICacheKeyBuilder<TKey, TKeyed>
    {
        public Func<TKeyed, TKey> Builder { get; set; }

        public TKey GetKey(TKeyed keyed)
        {
            #region Doer
            if (keyed == null) { throw new ArgumentNullException(nameof(keyed)); }
            #endregion

            return Builder(keyed);
        }
    }
}
