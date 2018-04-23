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
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

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
            Store.ChangeMonitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            return this;
        }

        public void MakeValue(ICacheValueBuilder<TKey, TValue> builder)
        {
            Store.ValueBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public void MakeValue(Func<TKey, TValue> builder)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            MakeValue(new FuncCacheValueBuilder<TKey, TValue>() { Builder = builder });
        }
    }

    /// <summary>
    /// CacheStore の設定を行うクラスです。
    /// </summary>
    internal sealed class CacheStoreConfiguration<TKeyed, TKey, TValue> : ICacheStoreConfiguration<TKeyed, TKey, TValue>, IMonitoringConfiguration<TKeyed, TKey, TValue>, IKeyConfiguration<TKeyed, TKey, TValue>, IValueConfiguration<TKeyed, TValue>
    {
        private CacheContext Context { get; set; }
        private CacheStore<TKeyed, TKey, TValue> Store { get; set; }
        private string StoreName { get; set; }

        public CacheStoreConfiguration(CacheContext context, string storeName)
        {
            Context = context;
            StoreName = storeName;
            Store = Context.StoreInstanceNamedMapping.Add<TKeyed, TKey, TValue>(Context.CenterType.FullName, StoreName);
        }

        public IMonitoringConfiguration<TKeyed, TKey, TValue> Settings(Action<ICacheStoreSettings> action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

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

        public IKeyConfiguration<TKeyed, TKey, TValue> WithDataSourceChangeMonitor(IDataSourceChangeMonitor<TKey> monitor)
        {
            Store.ChangeMonitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            return this;
        }

        public IValueConfiguration<TKeyed, TValue> MakeKey(Func<TKeyed, TKey> builder)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            Store.KeyBuilder = new FuncCacheKeyBuilder<TKeyed, TKey>() { Builder = builder };
            return this;
        }

        public void MakeValue(ICacheValueBuilder<TKeyed, TValue> builder)
        {
            Store.ValueBuilder = builder ?? throw new ArgumentNullException(nameof(builder));
        }

        public void MakeValue(Func<TKeyed, TValue> builder)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            MakeValue(new FuncCacheValueBuilder<TKeyed, TValue>() { Builder = builder });
        }
    }
}
