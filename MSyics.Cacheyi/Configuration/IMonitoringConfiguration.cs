/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Cacheyi.Monitoring;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// データソース監視の設定を行います。
    /// </summary>
    public interface IMonitoringConfiguration<TKey, TValue> : ICacheValueConfiguration<TKey, TValue>
    {
        /// <summary>
        /// データソースの変更通知を実装するオブジェクトを登録します。
        /// </summary>
        /// <param name="monitor">データソース変更通知を実装するオブジェクト</param>
        ICacheValueConfiguration<TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor);
    }

    /// <summary>
    /// データソース監視の設定を行います。
    /// </summary>
    public interface IMonitoringConfiguration<TKeyed, TKey, TValue> : ICacheKeyConfiguration<TKeyed, TKey, TValue>
    {
        /// <summary>
        /// データソースの変更通知を実装するオブジェクトを登録します。
        /// </summary>
        /// <param name="monitor">データソース変更通知を実装するオブジェクト</param>
        ICacheKeyConfiguration<TKeyed, TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor);
    }
}
