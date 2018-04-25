/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// キーについての設定を行います。
    /// </summary>
    public interface ICacheKeyConfiguration<TKeyed, TKey, TValue>
    {
        /// <summary>
        /// キャッシュオブジェクトを区別するためのキーを取得します。
        /// </summary>
        /// <param name="builder">キーを取得する機能を提供するデリゲート</param>
        ICacheValueConfiguration<TKey, TValue> GetKey(Func<TKeyed, TKey> builder);
    }
}
