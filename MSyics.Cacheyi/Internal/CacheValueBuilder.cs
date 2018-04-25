/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// データソースから指定したキーで値を取得する機能を提供します。
    /// </summary>
    /// <typeparam name="TKey">データソースから値を取得するためのキーの型</typeparam>
    /// <typeparam name="TValue">データソースから取得する値の型</typeparam>
    internal interface ICacheValueBuilder<TKey, TValue>
    {
/// <summary>
        /// 指定したキーでデータソースから値を取得します。
        /// </summary>
        /// <param name="key">データソースから値を取得するためのキー</param>
        TValue GetValue(TKey key);
    }

    /// <summary>
    /// デリゲートで CacheValue ビルダー を構築できるようにします。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal sealed class FuncCacheValueBuilder<TKey, TValue> : ICacheValueBuilder<TKey, TValue>
    {
        public Func<TKey, TValue> Build { get; set; }

        public TValue GetValue(TKey key)
        {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            return Build(key);
        }
    }
}
