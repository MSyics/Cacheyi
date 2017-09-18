/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Cacheyi.Monitors;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// データソース監視の設定を行います。
    /// </summary>
    public interface IMonitoringConfiguration<TKey, TValue> : IValueConfiguration<TKey, TValue>
    {
        /// <summary>
        /// データソースの変更通知を実装するオブジェクトを登録します。
        /// </summary>
        /// <param name="monitor">変更通知を実装するオブジェクト</param>
        IValueConfiguration<TKey, TValue> WithDataSourceChangeMonitor(IDataSourceChangeMonitor<TKey> monitor);
    }

    /// <summary>
    /// データソース監視の設定を行います。
    /// </summary>
    public interface IMonitoringConfiguration<TUnique, TKey, TValue> : IUniqueKeyConfiguration<TUnique, TKey, TValue>
    {
        /// <summary>
        /// データソースの変更通知を実装するオブジェクトを登録します。
        /// </summary>
        /// <param name="monitor">変更通知を実装するオブジェクト</param>
        IUniqueKeyConfiguration<TUnique, TKey, TValue> WithDataSourceChangeMonitor(IDataSourceChangeMonitor<TKey> monitor);
    }
}
