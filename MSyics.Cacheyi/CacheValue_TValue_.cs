/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュオブジェクトを表します。
    /// </summary>
    public sealed class CacheValue<TValue>
    {
        internal CacheValue() { }

        /// <summary>
        /// キャッシュしたオブジェクトを取得します。
        /// </summary>
        public TValue Value { get; internal set; }

        /// <summary>
        /// キャッシュした時刻を取得します。
        /// </summary>
        public DateTimeOffset Cached { get; internal set; }
    }
}
