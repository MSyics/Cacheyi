using System.Collections.Concurrent;

namespace MSyics.Cacheyi;

internal class CacheStoreCollection
{
    private readonly ConcurrentDictionary<string, object> stores = new();

    public object GetValue(string name)
    {
        if (!stores.TryGetValue(name, out var store)) { throw new KeyNotFoundException(name); }
        return store;
    }

    public void AddCacheStore<TKey, TValue>(string name) => stores.GetOrAdd(name, new CacheStore<TKey, TValue>());

    public void AddCacheStore<TKeyed, TKey, TValue>(string name) => stores.GetOrAdd(name, new CacheStore<TKeyed, TKey, TValue>());

    public void AddAsyncCacheStore<TKey, TValue>(string name) => stores.GetOrAdd(name, new AsyncCacheStore<TKey, TValue>());

    public void AddAsyncCacheStore<TKeyed, TKey, TValue>(string name) => stores.GetOrAdd(name, new AsyncCacheStore<TKeyed, TKey, TValue>());

    public CacheStore<TKey, TValue> GetCacheStore<TKey, TValue>(string name) => (CacheStore<TKey, TValue>)GetValue(name);

    public CacheStore<TKeyed, TKey, TValue> GetCacheStore<TKeyed, TKey, TValue>(string name) => (CacheStore<TKeyed, TKey, TValue>)GetValue(name);

    public AsyncCacheStore<TKey, TValue> GetAsyncCacheStore<TKey, TValue>(string name) => (AsyncCacheStore<TKey, TValue>)GetValue(name);

    public AsyncCacheStore<TKeyed, TKey, TValue> GetAsyncCacheStore<TKeyed, TKey, TValue>(string name) => (AsyncCacheStore<TKeyed, TKey, TValue>)GetValue(name);
}
