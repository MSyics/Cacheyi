/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Cacheyi.Configuration;
using MSyics.Cacheyi.Monitoring;
using System;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// CacheStore の設定を行うクラスです。
    /// </summary>
    internal sealed class CacheStoreConfiguration<TKey, TValue> : ICacheStoreConfiguration<TKey, TValue>, IMonitoringConfiguration<TKey, TValue>, ICacheValueConfiguration<TKey, TValue>
    {
        private CacheContext Context { get; set; }
        private CacheStore<TKey, TValue> Store { get; set; }
        private string StoreName { get; set; }

        public CacheStoreConfiguration(CacheContext context, string storeName)
        {
            Context = context;
            StoreName = storeName;
            Store = Context.Stores.Add<TKey, TValue>(Context.CenterType.FullName, StoreName);
        }

        public IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            var setting = new CacheStoreSettings()
            {
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

        public ICacheValueConfiguration<TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor)
        {
            Store.Monitoring = monitor ?? throw new ArgumentNullException(nameof(monitor));
            Store.Monitoring.Changed += Store.Internal.OnDataSourceChanged;
            return this;
        }

        public void GetValue(Func<TKey, TValue> builder)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            Store.Internal.ValueBuilder = new FuncCacheValueBuilder<TKey, TValue>() { Build = builder };
        }
    }

    /// <summary>
    /// CacheStore の設定を行うクラスです。
    /// </summary>
    internal sealed class CacheStoreConfiguration<TKeyed, TKey, TValue> : ICacheStoreConfiguration<TKeyed, TKey, TValue>, IMonitoringConfiguration<TKeyed, TKey, TValue>, ICacheKeyConfiguration<TKeyed, TKey, TValue>, ICacheValueConfiguration<TKey, TValue>
    {
        private CacheContext Context { get; set; }
        private CacheStore<TKeyed, TKey, TValue> Store { get; set; }
        private string StoreName { get; set; }

        public CacheStoreConfiguration(CacheContext context, string storeName)
        {
            Context = context;
            StoreName = storeName;
            Store = Context.Stores.Add<TKeyed, TKey, TValue>(Context.CenterType.FullName, StoreName);
        }

        public IMonitoringConfiguration<TKeyed, TKey, TValue> Settings(Action<ICacheStoreSettings> action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            var setting = new CacheStoreSettings()
            {
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

        public ICacheKeyConfiguration<TKeyed, TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor)
        {
            Store.Monitoring = monitor ?? throw new ArgumentNullException(nameof(monitor));
            return this;
        }

        public ICacheValueConfiguration<TKey, TValue> GetKey(Func<TKeyed, TKey> builder)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            Store.KeyBuilder = new FuncCacheKeyFactory<TKeyed, TKey>() { Build = builder };
            return this;
        }

        public void GetValue(Func<TKey, TValue> builder)
        {
            if (builder == null) { throw new ArgumentNullException(nameof(builder)); }
            Store.Internal.ValueBuilder = new FuncCacheValueBuilder<TKey, TValue>() { Build = builder };
        }
    }
}
