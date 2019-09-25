using System;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// 保持する要素を表します。
    /// </summary>
    public sealed class CacheValue<TValue>
    {
        internal CacheValue() { }

        /// <summary>
        /// 要素を取得します。
        /// </summary>
        public TValue Value { get; internal set; }

        /// <summary>
        /// 保持した時間を取得します。
        /// </summary>
        public DateTimeOffset Cached { get; internal set; }
    }
}
