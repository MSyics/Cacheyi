using MSyics.Cacheyi.Monitoring;

namespace MSyics.Cacheyi.Configuration;

/// <summary>
/// データソース監視の設定を行います。
/// </summary>
public interface IMonitoringConfiguration<TKey, TValue> : ICacheValueConfiguration<TKey, TValue>
{
    /// <summary>
    /// データソース監視機能を実装するオブジェクトを登録します。
    /// </summary>
    /// <param name="monitor">データソース監視機能を実装するオブジェクト</param>
    ICacheValueConfiguration<TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor);
}

/// <summary>
/// データソース監視の設定を行います。
/// </summary>
public interface IMonitoringConfiguration<TKeyed, TKey, TValue> : ICacheKeyConfiguration<TKeyed, TKey, TValue>
{
    /// <summary>
    /// データソース監視機能を実装するオブジェクトを登録します。
    /// </summary>
    /// <param name="monitor">データソース監視機能を実装するオブジェクト</param>
    ICacheKeyConfiguration<TKeyed, TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor);
}
