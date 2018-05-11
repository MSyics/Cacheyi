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
        private CacheStore<TKey, TValue> Store;

        public CacheStoreConfiguration(string name)
        {
            Store = new CacheContext().Stores.Add<TKey, TValue>(name);
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
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            Store.Internal.ValueBuilder = new FuncCacheValueBuilder<TKey, TKey, TValue>()
            {
                Build = (_, key) => builder(key)
            };
        }
    }

    internal sealed class CacheStoreConfiguration<TKeyed, TKey, TValue> : ICacheStoreConfiguration<TKeyed, TKey, TValue>, IMonitoringConfiguration<TKeyed, TKey, TValue>, ICacheKeyConfiguration<TKeyed, TKey, TValue>, ICacheValueConfiguration<TKeyed, TKey, TValue>
    {
        private CacheStore<TKeyed, TKey, TValue> Store;

        public CacheStoreConfiguration(string name)
        {
            Store = new CacheContext().Stores.Add<TKeyed, TKey, TValue>(name);
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

        ICacheValueConfiguration<TKeyed, TKey, TValue> ICacheKeyConfiguration<TKeyed, TKey, TValue>.GetKey(Func<TKeyed, TKey> builder)
        {
            Store.Internal.KeyBuilder = new FuncCacheKeyFactory<TKeyed, TKey>()
            {
                Build = builder ?? throw new ArgumentNullException(nameof(builder))
            };
            return this;
        }

        public void GetValue(Func<TKeyed, TKey, TValue> builder)
        {
            Store.Internal.ValueBuilder = new FuncCacheValueBuilder<TKeyed, TKey, TValue>()
            {
                Build = builder ?? throw new ArgumentNullException(nameof(builder))
            };
        }
    }
}
