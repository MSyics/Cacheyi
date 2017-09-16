/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// CacheStore についての設定を行います。
    /// </summary>
    public interface ICacheStoreConfiguration<TKey, TValue> : IMonitoringConfiguration<TKey, TValue>
    {
        /// <summary>
        /// CacheStore オブジェクトの設定を行います。
        /// </summary>
        /// <param name="action">設定オブジェクト</param>
        IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action);
    }

    /// <summary>
    /// CacheStore についての設定を行います。
    /// </summary>
    public interface ICacheStoreConfiguration<TUnique, TKey, TValue> : IMonitoringConfiguration<TUnique, TKey, TValue>
    {
        /// <summary>
        /// CacheStore オブジェクトの設定を行います。
        /// </summary>
        /// <param name="action">設定オブジェクト</param>
        IMonitoringConfiguration<TUnique, TKey, TValue> Settings(Action<ICacheStoreSettings> action);
    }
}
