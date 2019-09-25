using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// CacheStore の設定を行います。
    /// </summary>
    public interface ICacheStoreConfiguration<TKey, TValue> : IMonitoringConfiguration<TKey, TValue>
    {
        /// <summary>
        /// CacheStore の設定を行います。
        /// </summary>
        /// <param name="action">CasheStore の設定を行うための機能</param>
        IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action);
    }

    /// <summary>
    /// CacheStore の設定を行います。
    /// </summary>
    public interface ICacheStoreConfiguration<TKeyed, TKey, TValue> : IMonitoringConfiguration<TKeyed, TKey, TValue>
    {
        /// <summary>
        /// CacheStore の設定を行います。
        /// </summary>
        /// <param name="action">CasheStore の設定を行うための機能</param>
        IMonitoringConfiguration<TKeyed, TKey, TValue> Settings(Action<ICacheStoreSettings> action);
    }
}
