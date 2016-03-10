/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Cacheyi.Configuration;
using MSyics.Cacheyi.Monitors;
using System;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// CacheStore の設定を行うクラスです。
    /// </summary>
    internal sealed class CacheStoreConfiguration<TKey, TValue> : ICacheStoreConfiguration<TKey, TValue>, IMonitoringConfiguration<TKey, TValue>, IUniqueKeyConfiguration<TKey, TValue>, IValueConfiguration<TKey, TValue>
    {
        private CacheContext m_context;
        private CacheStore<TKey, TValue> m_store;
        private string m_storeName;

        public CacheStoreConfiguration(CacheContext context, string storeName)
        {
            this.m_context = context;
            this.m_storeName = storeName;
            this.m_store = this.m_context.StoreInstanceNamedMapping.Add<TKey, TValue>(this.m_context.CenterType.FullName, this.m_storeName);
        }

        public IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action)
        {
            #region Doer
            if (action == null) { throw new ArgumentNullException(nameof(action)); }
            #endregion

            var setting = new CacheStoreSettings()
            {
                CenterType = this.m_context.CenterType,
                StoreName = this.m_storeName,
                MaxCapacity = 0,
                Timeout = TimeSpan.Zero,
            };

            action(setting);
            if (setting.MaxCapacity.HasValue)
            {
                m_store.MaxCapacity = setting.MaxCapacity.Value;
            }
            if (setting.Timeout.HasValue)
            {
                m_store.Timeout = setting.Timeout.Value;
            }

            return this;
        }

        public IUniqueKeyConfiguration<TKey, TValue> WithDataSourceChangeMonitor(IDataSourceChangeMonitor<TKey> monitor)
        {
            #region Doer
            if (monitor == null) { throw new ArgumentNullException(nameof(monitor)); }
            #endregion

            m_store.ChangeMonitor = monitor;
            return this;
        }

        public IValueConfiguration<TKey, TValue> MakeUniqueKey(ICacheKeyBuilder<TKey> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            m_store.KeyBuilder = builder;
            return this;
        }

        public IValueConfiguration<TKey, TValue> MakeUniqueKey(Func<TKey, int> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            this.MakeUniqueKey(new FuncCacheKeyBuilder<TKey>() { Builder = builder });
            return this;
        }

        public void MakeValue(ICacheValueBuilder<TKey, TValue> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            m_store.ValueBuilder = builder;
        }

        public void MakeValue(Func<TKey, TValue> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            MakeValue(new FuncCacheValueBuilder<TKey, TValue>() { Builder = builder });
        }
    }
}
