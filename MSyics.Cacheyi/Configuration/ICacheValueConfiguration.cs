/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// キャッシュする値についての設定をします。
    /// </summary>
    public interface ICacheValueConfiguration<TKey, TValue>
    {
        /// <summary>
        /// キャッシュするオブジェクトを取得します。
        /// </summary>
        /// <param name="builder">指定したキーからキャッシュするオブジェクトを取得する機能を提供するデリゲート</param>
        void GetValue(Func<TKey, TValue> builder);
    }
}
