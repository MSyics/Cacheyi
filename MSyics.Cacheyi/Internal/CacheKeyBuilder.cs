/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュ関連オブジェクトおよびオブジェクトを区別するキーを派生します。
    /// </summary>
    /// <typeparam name="TKeyed"></typeparam>
    internal interface ICacheKeyBuilder<TKeyed, TKey>
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
    internal sealed class FuncCacheKeyFactory<TKeyed, TKey> : ICacheKeyBuilder<TKeyed, TKey>
    {
        public Func<TKeyed, TKey> Build { get; set; }

        public TKey GetKey(TKeyed keyed)
        {
            if (keyed == null) { throw new ArgumentNullException(nameof(keyed)); }
            return Build(keyed);
        }
    }
}
