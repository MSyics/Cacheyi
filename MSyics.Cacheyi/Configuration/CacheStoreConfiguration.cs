using MSyics.Cacheyi.Configuration;
using MSyics.Cacheyi.Monitoring;

namespace MSyics.Cacheyi;

internal static class CacheStoreConfiguration
{
    public static void Settings<TKeyed, TKey, TValue>(ref IInternalAsyncCacheStore<TKeyed, TKey, TValue> store, Action<ICacheStoreSettings> action)
    {
        var setting = new CacheStoreSettings();
        action?.Invoke(setting);
        store.MaxCapacity = setting.MaxCapacity ?? 0;
        store.Timeout = setting.Timeout ?? TimeSpan.Zero;
        store.TimeoutBehaivor = setting.TimeoutBehavior ?? CacheTimeoutBehaivor.Reset;
    }

    public static void WithMonitoring<TKeyed, TKey, TValue>(ref IInternalAsyncCacheStore<TKeyed, TKey, TValue> store, IDataSourceMonitoring<TKey> monitor)
    {
        store.Monitoring = monitor;
        if (store.Monitoring is not null)
        {
            store.Monitoring.DataSourceChanged += store.OnDataSourceChanged;
        }
    }

    public static void GetKey<TKeyed, TKey, TValue>(ref IInternalAsyncCacheStore<TKeyed, TKey, TValue> store, Func<TKeyed, TKey> builder)
    {
        store.KeyBuilder = new FuncCacheKeyFactory<TKeyed, TKey>
        {
            Build = builder
        };
    }

    public static void GetValue<TKeyed, TKey, TValue>(ref IInternalAsyncCacheStore<TKeyed, TKey, TValue> store, Func<TKeyed, CancellationToken, TValue> builder)
    {
        store.ValueBuilder = new FuncCacheValueBuilder<TKeyed, TKey, TValue>()
        {
            Build = builder
        };
    }
}

internal class CacheStoreConfiguration<TKeyed, TKey, TValue> :
    ICacheStoreConfiguration<TKeyed, TKey, TValue>,
    IMonitoringConfiguration<TKeyed, TKey, TValue>,
    ICacheKeyConfiguration<TKeyed, TKey, TValue>,
    ICacheValueConfiguration<TKeyed, TKey, TValue>
{
    private readonly string name;

    public CacheStoreConfiguration(string name)
    {
        this.name = name;
        AddCacheStore(name, CacheCenter.stores);
    }

    public IMonitoringConfiguration<TKeyed, TKey, TValue> Settings(Action<ICacheStoreSettings> action)
    {
        var store = GetCacheStore(name, CacheCenter.stores);
        CacheStoreConfiguration.Settings(ref store, action);
        return this;
    }

    public ICacheKeyConfiguration<TKeyed, TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor)
    {
        var store = GetCacheStore(name, CacheCenter.stores);
        CacheStoreConfiguration.WithMonitoring(ref store, monitor);
        return this;
    }

    public ICacheValueConfiguration<TKeyed, TKey, TValue> GetKey(Func<TKeyed, TKey> builder)
    {
        var store = GetCacheStore(name, CacheCenter.stores);
        CacheStoreConfiguration.GetKey(ref store, builder);
        return this;
    }

    ICacheValueConfiguration<TKeyed, TKey, TValue> ICacheKeyConfiguration<TKeyed, TKey, TValue>.GetKey(Func<TKeyed, TKey> builder) => GetKey(builder);

    public void GetValue(Func<TKeyed, CancellationToken, TValue> builder)
    {
        var store = GetCacheStore(name, CacheCenter.stores);
        CacheStoreConfiguration.GetValue(ref store, builder);
    }

    protected virtual void AddCacheStore(string name, CacheStoreCollection stores) =>
        stores.AddCacheStore<TKeyed, TKey, TValue>(name);

    protected virtual IInternalAsyncCacheStore<TKeyed, TKey, TValue> GetCacheStore(string name, CacheStoreCollection stores) =>
        stores.GetCacheStore<TKeyed, TKey, TValue>(name).Internal;
}

internal class CacheStoreConfiguration<TKey, TValue> :
    ICacheStoreConfiguration<TKey, TValue>,
    IMonitoringConfiguration<TKey, TValue>,
    ICacheValueConfiguration<TKey, TValue>
{
    private readonly string name;

    public CacheStoreConfiguration(string name)
    {
        this.name = name;
        AddCacheStore(name, CacheCenter.stores);
    }

    public IMonitoringConfiguration<TKey, TValue> Settings(Action<ICacheStoreSettings> action)
    {
        var store = GetCacheStore(name, CacheCenter.stores);
        CacheStoreConfiguration.Settings(ref store, action);
        return this;
    }

    public ICacheValueConfiguration<TKey, TValue> WithMonitoring(IDataSourceMonitoring<TKey> monitor)
    {
        var store = GetCacheStore(name, CacheCenter.stores);
        CacheStoreConfiguration.WithMonitoring(ref store, monitor);
        return this;
    }

    public void GetValue(Func<TKey, CancellationToken, TValue> builder)
    {
        var store = GetCacheStore(name, CacheCenter.stores);
        CacheStoreConfiguration.GetValue(ref store, builder);
    }

    protected virtual void AddCacheStore(string name, CacheStoreCollection stores) =>
        stores.AddCacheStore<TKey, TValue>(name);

    protected virtual IInternalAsyncCacheStore<TKey, TKey, TValue> GetCacheStore(string name, CacheStoreCollection stores) =>
        stores.GetCacheStore<TKey, TValue>(name).Internal;
}

internal sealed class AsyncCacheStoreConfiguration<TKeyed, TKey, TValue> : CacheStoreConfiguration<TKeyed, TKey, Task<TValue>>
{
    public AsyncCacheStoreConfiguration(string name) : base(name) { }

    protected override void AddCacheStore(string name, CacheStoreCollection stores) =>
      stores.AddAsyncCacheStore<TKeyed, TKey, TValue>(name);

    protected override IInternalAsyncCacheStore<TKeyed, TKey, Task<TValue>> GetCacheStore(string name, CacheStoreCollection stores) =>
        stores.GetAsyncCacheStore<TKeyed, TKey, TValue>(name).Internal;
}

internal sealed class AsyncCacheStoreConfiguration<TKey, TValue> : CacheStoreConfiguration<TKey, Task<TValue>>
{
    public AsyncCacheStoreConfiguration(string name) : base(name) { }

    protected override void AddCacheStore(string name, CacheStoreCollection stores) =>
        stores.AddAsyncCacheStore<TKey, TValue>(name);

    protected override IInternalAsyncCacheStore<TKey, TKey, Task<TValue>> GetCacheStore(string name, CacheStoreCollection stores) =>
        stores.GetAsyncCacheStore<TKey, TValue>(name).Internal;
}
