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
    internal sealed class CacheStoreConfiguration<TKey, TValue> : ICacheStoreConfiguration<TKey, TValue>, IMonitoringConfiguration<TKey, TValue>, ICacheValueConfiguration<TKey, TValue>
    {
        private CacheContext Context;
        private CacheStore<TKey, TValue> Store;

        public CacheStoreConfiguration(CacheContext context, string storeName)
        {
            Context = context;
            Store = Context.Stores.Add<TKey, TValue>(Context.CenterType.FullName, storeName);
        }

        public IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action)
        {
            var setting = new CacheStoreSettings();
            action?.Invoke(setting);
            Store.MaxCapacity = setting.MaxCapacity ?? 0;
            Store.Timeout = setting.Timeout ?? TimeSpan.Zero;
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
            Store.Internal.ValueBuilder = new FuncCacheValueBuilder<TKey, TValue>()
            {
                Build = builder ?? throw new ArgumentNullException(nameof(builder))
            };
        }
    }

    internal sealed class CacheStoreConfiguration<TKeyed, TKey, TValue> : ICacheStoreConfiguration<TKeyed, TKey, TValue>, IMonitoringConfiguration<TKeyed, TKey, TValue>, ICacheKeyConfiguration<TKeyed, TKey, TValue>, ICacheValueConfiguration<TKey, TValue>
    {
        private CacheContext Context;
        private CacheStore<TKeyed, TKey, TValue> Store;

        public CacheStoreConfiguration(CacheContext context, string storeName)
        {
            Context = context;
            Store = Context.Stores.Add<TKeyed, TKey, TValue>(Context.CenterType.FullName, storeName);
        }

        public IMonitoringConfiguration<TKeyed, TKey, TValue> Settings(Action<ICacheStoreSettings> action)
        {
            var setting = new CacheStoreSettings();
            action?.Invoke(setting);
            Store.MaxCapacity = setting.MaxCapacity ?? 0;
            Store.Timeout = setting.Timeout ?? TimeSpan.Zero;
            return this;
        }

        public ICacheKeyConfiguration<TKeyed, TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor)
        {
            Store.Monitoring = monitor ?? throw new ArgumentNullException(nameof(monitor));
            Store.Monitoring.Changed += Store.Internal.OnDataSourceChanged;
            return this;
        }

        public ICacheValueConfiguration<TKey, TValue> GetKey(Func<TKeyed, TKey> builder)
        {
            Store.KeyBuilder = new FuncCacheKeyFactory<TKeyed, TKey>()
            {
                Build = builder ?? throw new ArgumentNullException(nameof(builder))
            };
            return this;
        }

        public void GetValue(Func<TKey, TValue> builder)
        {
            Store.Internal.ValueBuilder = new FuncCacheValueBuilder<TKey, TValue>()
            {
                Build = builder ?? throw new ArgumentNullException(nameof(builder))
            };
        }
    }
}
