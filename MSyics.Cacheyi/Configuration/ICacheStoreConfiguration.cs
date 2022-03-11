using MSyics.Cacheyi.Monitoring;

namespace MSyics.Cacheyi.Configuration;

/// <summary>
/// CacheStore の設定を行います。
/// </summary>
public interface ICacheStoreConfiguration<TKey, TValue>
{
    /// <summary>
    /// CacheStore の設定を行います。
    /// </summary>
    /// <param name="action">CasheStore の設定を行うための機能</param>
    IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action);

    /// <summary>
    /// データソース監視機能を実装するオブジェクトを登録します。
    /// </summary>
    /// <param name="monitor">データソース監視機能を実装するオブジェクト</param>
    ICacheValueConfiguration<TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor);

    /// <summary>
    /// 保持する要素を取得します。
    /// </summary>
    /// <param name="builder">指定したキーから保持する要素を取得する機能</param>
    void GetValue(Func<TKey, CancellationToken, TValue> builder);
}

/// <summary>
/// CacheStore の設定を行います。
/// </summary>
public interface ICacheStoreConfiguration<TKeyed, TKey, TValue>
{
    /// <summary>
    /// CacheStore の設定を行います。
    /// </summary>
    /// <param name="action">CasheStore の設定を行うための機能</param>
    IMonitoringConfiguration<TKeyed, TKey, TValue> Settings(Action<ICacheStoreSettings> action);

    /// <summary>
    /// データソース監視機能を実装するオブジェクトを登録します。
    /// </summary>
    /// <param name="monitor">データソース監視機能を実装するオブジェクト</param>
    ICacheKeyConfiguration<TKeyed, TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor);

    /// <summary>
    /// 保持する要素を選別するキーを取得します。
    /// </summary>
    /// <param name="builder">指定した要素からキーを取得する機能</param>
    ICacheValueConfiguration<TKeyed, TKey, TValue> GetKey(Func<TKeyed, TKey> builder);
}

/// <summary>
/// データソース監視の設定を行います。
/// </summary>
public interface IMonitoringConfiguration<TKey, TValue>
{
    /// <summary>
    /// データソース監視機能を実装するオブジェクトを登録します。
    /// </summary>
    /// <param name="monitor">データソース監視機能を実装するオブジェクト</param>
    ICacheValueConfiguration<TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor);

    /// <summary>
    /// 保持する要素を取得します。
    /// </summary>
    /// <param name="builder">指定したキーから保持する要素を取得する機能</param>
    void GetValue(Func<TKey, CancellationToken, TValue> builder);
}

/// <summary>
/// データソース監視の設定を行います。
/// </summary>
public interface IMonitoringConfiguration<TKeyed, TKey, TValue>
{
    /// <summary>
    /// データソース監視機能を実装するオブジェクトを登録します。
    /// </summary>
    /// <param name="monitor">データソース監視機能を実装するオブジェクト</param>
    ICacheKeyConfiguration<TKeyed, TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor);

    /// <summary>
    /// 保持する要素を選別するキーを取得します。
    /// </summary>
    /// <param name="builder">指定した要素からキーを取得する機能</param>
    ICacheValueConfiguration<TKeyed, TKey, TValue> GetKey(Func<TKeyed, TKey> builder);
}

/// <summary>
/// 保持する要素を選別するキーの設定を行います。
/// </summary>
public interface ICacheKeyConfiguration<TKeyed, TKey, TValue>
{
    /// <summary>
    /// 保持する要素を選別するキーを取得します。
    /// </summary>
    /// <param name="builder">指定した要素からキーを取得する機能</param>
    ICacheValueConfiguration<TKeyed, TKey, TValue> GetKey(Func<TKeyed, TKey> builder);
}

/// <summary>
/// 保持する要素の設定を行います。
/// </summary>
public interface ICacheValueConfiguration<TKey, TValue>
{
    /// <summary>
    /// 保持する要素を取得します。
    /// </summary>
    /// <param name="builder">指定したキーから保持する要素を取得する機能</param>
    void GetValue(Func<TKey, CancellationToken, TValue> builder);
}

/// <summary>
/// 保持する要素の設定を行います。
/// </summary>
public interface ICacheValueConfiguration<TKeyed, TKey, TValue>
{
    /// <summary>
    /// 保持する要素を取得します。
    /// </summary>
    /// <param name="builder">指定したキーから保持する要素を取得する機能</param>
    void GetValue(Func<TKeyed, CancellationToken, TValue> builder);
}
