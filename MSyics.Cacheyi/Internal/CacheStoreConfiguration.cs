/****************************************************************
© 2018 MSyics
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
    internal sealed class CacheStoreConfiguration<TKey, TValue> : ICacheStoreConfiguration<TKey, TValue>, IMonitoringConfiguration<TKey, TValue>, IValueConfiguration<TKey, TValue>
    {
        private CacheContext Context { get; set; }
        private CacheStore<TKey, TValue> Store { get; set; }
        private string StoreName { get; set; }

        public CacheStoreConfiguration(CacheContext context, string storeName)
        {
            Context = context;
            StoreName = storeName;
            Store = Context.StoreInstanceNamedMapping.Add<TKey, TValue>(Context.CenterType.FullName, StoreName);
            Store.KeyBuilder = new FuncCacheKeyBuilder<TKey, TKey>() { Builder = key => key };
        }

        public IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action)
        {
            #region Doer
            if (action == null) { throw new ArgumentNullException(nameof(action)); }
            #endregion

            var setting = new CacheStoreSettings()
            {
                CenterType = Context.CenterType,
                StoreName = StoreName,
                MaxCapacity = 0,
                Timeout = TimeSpan.Zero,
            };

            action(setting);
            if (setting.MaxCapacity.HasValue)
            {
                Store.MaxCapacity = setting.MaxCapacity.Value;
            }
            if (setting.Timeout.HasValue)
            {
                Store.Timeout = setting.Timeout.Value;
            }

            return this;
        }

        public IValueConfiguration<TKey, TValue> WithDataSourceChangeMonitor(IDataSourceChangeMonitor<TKey> monitor)
        {
            #region Doer
            if (monitor == null) { throw new ArgumentNullException(nameof(monitor)); }
            #endregion

            Store.ChangeMonitor = monitor;
            return this;
        }

        public void MakeValue(ICacheValueBuilder<TKey, TValue> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            Store.ValueBuilder = builder;
        }

        public void MakeValue(Func<TKey, TValue> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            MakeValue(new FuncCacheValueBuilder<TKey, TValue>() { Builder = builder });
        }
    }

    /// <summary>
    /// CacheStore の設定を行うクラスです。
    /// </summary>
    internal sealed class CacheStoreConfiguration<TKey, TKeyed, TValue> : ICacheStoreConfiguration<TKey, TKeyed, TValue>, IMonitoringConfiguration<TKey, TKeyed, TValue>, IKeyConfiguration<TKey, TKeyed, TValue>, IValueConfiguration<TKeyed, TValue>
    {
        private CacheContext Context { get; set; }
        private CacheStore<TKey, TKeyed, TValue> Store { get; set; }
        private string StoreName { get; set; }

        public CacheStoreConfiguration(CacheContext context, string storeName)
        {
            Context = context;
            StoreName = storeName;
            Store = Context.StoreInstanceNamedMapping.Add<TKey, TKeyed, TValue>(Context.CenterType.FullName, StoreName);
        }

        public IMonitoringConfiguration<TKey, TKeyed, TValue> Settings(Action<ICacheStoreSettings> action)
        {
            #region Doer
            if (action == null) { throw new ArgumentNullException(nameof(action)); }
            #endregion

            var setting = new CacheStoreSettings()
            {
                CenterType = Context.CenterType,
                StoreName = StoreName,
                MaxCapacity = 0,
                Timeout = TimeSpan.Zero,
            };

            action(setting);
            if (setting.MaxCapacity.HasValue)
            {
                Store.MaxCapacity = setting.MaxCapacity.Value;
            }
            if (setting.Timeout.HasValue)
            {
                Store.Timeout = setting.Timeout.Value;
            }

            return this;
        }

        public IKeyConfiguration<TKey, TKeyed, TValue> WithDataSourceChangeMonitor(IDataSourceChangeMonitor<TKeyed> monitor)
        {
            #region Doer
            if (monitor == null) { throw new ArgumentNullException(nameof(monitor)); }
            #endregion

            Store.ChangeMonitor = monitor;
            return this;
        }

        public IValueConfiguration<TKeyed, TValue> MakeUniqueKey(ICacheKeyBuilder<TKey, TKeyed> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            Store.KeyBuilder = builder;
            return this;
        }

        public IValueConfiguration<TKeyed, TValue> MakeKey(Func<TKeyed, TKey> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            this.MakeUniqueKey(new FuncCacheKeyBuilder<TKey, TKeyed>() { Builder = builder });
            return this;
        }

        public void MakeValue(ICacheValueBuilder<TKeyed, TValue> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            Store.ValueBuilder = builder;
        }

        public void MakeValue(Func<TKeyed, TValue> builder)
        {
            #region Doer
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            #endregion

            MakeValue(new FuncCacheValueBuilder<TKeyed, TValue>() { Builder = builder });
        }


    }
}
