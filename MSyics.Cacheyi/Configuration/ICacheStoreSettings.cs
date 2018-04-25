/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// CacheStore の設定値を表します。
    /// </summary>
    public interface ICacheStoreSettings
    {
        /// <summary>
        /// キャッシュできる最大容量を取得または設定します。
        /// </summary>
        int? MaxCapacity { get; set; }

        /// <summary>
        /// キャッシュの保持期間を取得または設定します。
        /// </summary>
        TimeSpan? Timeout { get; set; }
    }

    internal class CacheStoreSettings : ICacheStoreSettings
    {
        public int? MaxCapacity { get; set; }
        public TimeSpan? Timeout { get; set; }
    }
}
