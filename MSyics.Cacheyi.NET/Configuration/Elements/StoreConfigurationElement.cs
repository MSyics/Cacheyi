/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Configuration;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// Store 要素を表します。
    /// </summary>
    internal class StoreConfigurationElement : ConfigurationElement, ICacheStoreSettings
    {
        #region Const Members

        /// <summary>
        /// MaxCapacity プロパティの初期値を示す定数です。
        /// </summary>
        public const int DefaultMaxCapacity = 0;

        /// <summary>
        /// Timeout プロパティの初期値を示す定数です。
        /// </summary>
        public const string DefaultTimeout = "00:00:00";

        /// <summary>
        /// PerformanceCounters プロパティの初期値を示す定数です。
        /// </summary>
        public const bool DefaultPerformanceCounter = false;

        #endregion // End Const Members

        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string StoreName
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// キャッシュできる最大容量を取得または設定します。
        /// </summary>
        [ConfigurationProperty("maxCapacity")]
        public int? MaxCapacity
        {
            get { return (int?)this["maxCapacity"]; }
            set { this["maxCapacity"] = value; }
        }

        /// <summary>
        /// キャッシュの保持期間を取得または設定します。
        /// </summary>
        [ConfigurationProperty("timeout")]
        public TimeSpan? Timeout
        {
            get { return (TimeSpan?)this["timeout"]; }
            set { this["timeout"] = value; }
        }

        ///// <summary>
        ///// パフォーマンスカウンターを有効にするかどうかを示す値を取得または設定します。
        ///// </summary>
        //[ConfigurationProperty("performanceCounters")]
        //public bool? PerformanceCounters
        //{
        //    get { return (bool?)this["performanceCounters"]; }
        //    set { this["performanceCounters"] = value; }
        //}

        public Type CenterType { get; }
    }
}
