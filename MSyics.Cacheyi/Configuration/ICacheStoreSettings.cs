/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// キャッシュストアを構成するための設定値を表します。
    /// </summary>
    public interface ICacheStoreSettings
    {
        /// <summary>
        /// CacheCenter の型を取得します。
        /// </summary>
        Type CenterType { get; }

        /// <summary>
        /// CacheStore の名前を取得します。
        /// </summary>
        string StoreName { get; }

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
        public Type CenterType { get; set; }

        public string StoreName { get; set; }

        public int? MaxCapacity { get; set; }

        public TimeSpan? Timeout { get; set; }
    }
}
