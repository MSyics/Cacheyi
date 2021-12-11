using MSyics.Cacheyi.Configuration;
using MSyics.Cacheyi.Monitoring;

namespace MSyics.Cacheyi;

internal sealed class CacheStoreConfiguration<TKey, TValue> :
    ICacheStoreConfiguration<TKey, TValue>,
    IMonitoringConfiguration<TKey, TValue>,
    ICacheValueConfiguration<TKey, TValue>
{
    private readonly string name;

    public CacheStoreConfiguration(string name)
    {
        this.name = name;
        CacheCenter.stores.Add<TKey, TValue>(name);
    }

    public IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action)
    {
        var setting = new CacheStoreSettings();
        action?.Invoke(setting);
        var store = CacheCenter.stores.GetValue<TKey, TValue>(name);
        store.MaxCapacity = setting.MaxCapacity ?? 0;
        store.Timeout = setting.Timeout ?? TimeSpan.Zero;
        store.TimeoutBehaivor = setting.TimeoutBehavior ?? CacheTimeoutBehaivor.Reset;
        return this;
    }

    public ICacheValueConfiguration<TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor)
    {
        var store = CacheCenter.stores.GetValue<TKey, TValue>(name);
        store.Monitoring = monitor;
        if (store.Monitoring is not null)
        {
            store.Monitoring.DataSourceChanged += store.Internal.OnDataSourceChanged;
        }
        return this;
    }

    public void GetValue(Func<TKey, TValue> builder)
    {
        var store = CacheCenter.stores.GetValue<TKey, TValue>(name);
        store.Internal.ValueBuilder = new FuncCacheValueBuilder<TKey, TKey, TValue>()
        {
            Build = key => builder(key)
        };
    }
}

internal sealed class CacheStoreConfiguration<TKeyed, TKey, TValue> :
    ICacheStoreConfiguration<TKeyed, TKey, TValue>,
    IMonitoringConfiguration<TKeyed, TKey, TValue>,
    ICacheKeyConfiguration<TKeyed, TKey, TValue>,
    ICacheValueConfiguration<TKeyed, TKey, TValue>
{
    private readonly string name;

    public CacheStoreConfiguration(string name)
    {
        this.name = name;
        CacheCenter.stores.Add<TKeyed, TKey, TValue>(name);
    }

    public IMonitoringConfiguration<TKeyed, TKey, TValue> Settings(Action<ICacheStoreSettings> action)
    {
        var setting = new CacheStoreSettings();
        action?.Invoke(setting);
        var store = CacheCenter.stores.GetValue<TKeyed, TKey, TValue>(name);
        store.MaxCapacity = setting.MaxCapacity ?? 0;
        store.Timeout = setting.Timeout ?? TimeSpan.Zero;
        store.TimeoutBehaivor = setting.TimeoutBehavior ?? CacheTimeoutBehaivor.Reset;
        return this;
    }

    public ICacheKeyConfiguration<TKeyed, TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor)
    {
        var store = CacheCenter.stores.GetValue<TKeyed, TKey, TValue>(name);
        store.Monitoring = monitor;
        if (store.Monitoring is not null)
        {
            store.Monitoring.DataSourceChanged += store.Internal.OnDataSourceChanged;
        }
        return this;
    }

    ICacheValueConfiguration<TKeyed, TKey, TValue> ICacheKeyConfiguration<TKeyed, TKey, TValue>.GetKey(Func<TKeyed, TKey> builder)
    {
        var store = CacheCenter.stores.GetValue<TKeyed, TKey, TValue>(name);
        store.Internal.KeyBuilder = new FuncCacheKeyFactory<TKeyed, TKey>
        {
            Build = builder
        };
        return this;
    }

    public void GetValue(Func<TKeyed, TValue> builder)
    {
        var store = CacheCenter.stores.GetValue<TKeyed, TKey, TValue>(name);
        store.Internal.ValueBuilder = new FuncCacheValueBuilder<TKeyed, TKey, TValue>()
        {
            Build = builder
        };
    }
}
