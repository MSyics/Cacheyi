using MSyics.Cacheyi.Monitoring;

namespace MSyics.Cacheyi;

/// <summary>
/// 要素を保持します。
/// </summary>
/// <typeparam name="TKeyed">要素のキーを保有する型</typeparam>
/// <typeparam name="TKey">要素を選別するキーの型</typeparam>
/// <typeparam name="TValue">要素の型</typeparam>
public interface IAsyncCacheStore<TKeyed, TKey, TValue>
{
    /// <summary>
    /// 要素の最大保持量を取得します。
    /// </summary>
    int MaxCapacity { get; set; }

    /// <summary>
    /// /最大保持量を持っているかどうかを示す値を取得します。
    /// </summary>
    bool HasMaxCapacity { get; }

    /// <summary>
    /// 要素の保持期間を取得します。
    /// </summary>
    TimeSpan Timeout { get; set; }

    /// <summary>
    /// 保持期間を持っているかどうかを示す値を取得します。
    /// </summary>
    bool HasTimeout { get; }

    /// <summary>
    /// 保持期間を過ぎた際の挙動を取得します。
    /// </summary>
    CacheTimeoutBehaivor TimeoutBehaivor { get; set; }

    /// <summary>
    /// データソース監視オブジェクトを取得します。
    /// </summary>
    IDataSourceMonitoring<TKey> Monitoring { get; }

    /// <summary>
    /// データソース監視ができるかどうかを示す値を取得します。
    /// </summary>
    bool CanMonitoring { get; }

    /// <summary>
    /// 要素の一覧を取得します。
    /// </summary>
    IEnumerable<AsyncCacheProxy<TKey, TValue>> AsEnumerable();

    /// <summary>
    /// 要素の保持数を取得します。
    /// </summary>
    int Count { get; }

    /// <summary>
    /// 保持している要素をすべて解除します。
    /// </summary>
    void Clear();

    /// <summary>
    /// <para>保持している要素を圧縮して整理します。</para>
    /// <para>この操作は、実要素を保持していない要素を解除します。</para>
    /// </summary>
    void TrimExcess();

    /// <summary>
    /// すべての要素をリセットします。
    /// </summary>
    Task ResetAsync();

    /// <summary>
    /// 指定したオブジェクトから要素を引き当てます。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    AsyncCacheProxy<TKey, TValue> Allocate(TKeyed keyed);

    /// <summary>
    /// 指定したオブジェクトの一覧から要素を引き当てます。
    /// </summary>
    /// <param name="keyeds">キーを保有するオブジェクトの一覧</param>
    AsyncCacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKeyed> keyeds);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    /// <param name="func">振替値を取得する処理</param>
    ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, Func<CancellationToken, Task<TValue>> func);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    /// <param name="func">振替値を取得する処理</param>
    ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, Func<Task<TValue>> func);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    /// <param name="value">振替値</param>
    ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, TValue value);

    /// <summary>
    /// 指定したオブジェクトで要素を解除します。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    bool Release(TKeyed keyed);

    /// <summary>
    /// 指定したオブジェクトの一覧から要素を解除します。
    /// </summary>
    /// <param name="keyeds">キーを保有するオブジェクトの一覧</param>
    void Release(IEnumerable<TKeyed> keyeds);
}

/// <summary>
/// 要素を保持します。
/// </summary>
/// <typeparam name="TKey">要素を選別するキーの型</typeparam>
/// <typeparam name="TValue">要素の型</typeparam>
public interface IAsyncCacheStore<TKey, TValue> : IAsyncCacheStore<TKey, TKey, TValue>
{
    /// <summary>
    /// 指定したキーから要素を引き当てます。
    /// </summary>
    /// <param name="key">キー</param>
    new AsyncCacheProxy<TKey, TValue> Allocate(TKey key);

    /// <summary>
    /// 指定したキーの一覧から要素を引き当てます。
    /// </summary>
    /// <param name="keys">キーの一覧</param>
    new AsyncCacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKey> keys);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="value">振替値</param>
    new ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKey key, TValue value);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="func">振替値を取得する処理</param>
    new ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKey key, Func<CancellationToken, Task<TValue>> func);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="func">振替値を取得する処理</param>
    new ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKey key, Func<Task<TValue>> func);

    /// <summary>
    /// 指定したキーで要素を解除します。
    /// </summary>
    /// <param name="key">キー</param>
    new bool Release(TKey key);

    /// <summary>
    /// 指定したキーの一覧から要素を解除します。
    /// </summary>
    /// <param name="keys">キーの一覧</param>
    new void Release(IEnumerable<TKey> keys);
}

/// <summary>
/// 要素を保持します。
/// </summary>
/// <typeparam name="TKey">要素を選別するキーの型</typeparam>
/// <typeparam name="TValue">要素の型</typeparam>
public sealed class AsyncCacheStore<TKey, TValue> : IAsyncCacheStore<TKey, TValue>
{
    internal InternalAsyncCacheStore<TKey, TKey, TValue> Internal { get; }

    internal AsyncCacheStore()
    {
        Internal = new InternalAsyncCacheStore<TKey, TKey, TValue>
        {
            KeyBuilder = new FuncCacheKeyFactory<TKey, TKey>() { Build = key => key }
        };
    }

    /// <inheritdoc/>
    public AsyncCacheStore(
        Func<TKey, CancellationToken, Task<TValue>> valueBuilder,
        IDataSourceMonitoring<TKey> monitoring = null,
        int? maxCapacity = null,
        TimeSpan? timeout = null,
        CacheTimeoutBehaivor? timeoutBehaivor = CacheTimeoutBehaivor.Reset)
    {
        Internal = new InternalAsyncCacheStore<TKey, TKey, TValue>(
            new FuncCacheKeyFactory<TKey, TKey>
            {
                Build = key => key
            },
            new FuncCacheValueBuilder<TKey, TKey, Task<TValue>>
            {
                Build = (key, token) => valueBuilder(key, token)
            },
            monitoring,
            maxCapacity,
            timeout,
            timeoutBehaivor);
    }

    /// <inheritdoc/>
    public int MaxCapacity { get => Internal.MaxCapacity; set => Internal.MaxCapacity = value; }

    /// <inheritdoc/>
    public bool HasMaxCapacity => Internal.HasMaxCapacity;

    /// <inheritdoc/>
    public TimeSpan Timeout { get => Internal.Timeout; set => Internal.Timeout = value; }

    /// <inheritdoc/>
    public bool HasTimeout => Internal.HasTimeout;

    /// <inheritdoc/>
    public CacheTimeoutBehaivor TimeoutBehaivor { get => Internal.TimeoutBehaivor; set => Internal.TimeoutBehaivor = value; }

    /// <inheritdoc/>
    public IDataSourceMonitoring<TKey> Monitoring { get => Internal.Monitoring; internal set => Internal.Monitoring = value; }

    /// <inheritdoc/>
    public bool CanMonitoring => Internal.CanMonitoring;

    /// <inheritdoc/>
    public IEnumerable<AsyncCacheProxy<TKey, TValue>> AsEnumerable() => Internal.AsEnumerable();

    /// <inheritdoc/>
    public int Count => Internal.Count;

    /// <inheritdoc/>
    public void Clear() => Internal.Clear();

    /// <inheritdoc/>
    public void TrimExcess() => Internal.TrimExcess();

    /// <inheritdoc/>
    public Task ResetAsync() => Internal.ResetAsync();

    /// <inheritdoc/>
    public AsyncCacheProxy<TKey, TValue> Allocate(TKey key) => Internal.Allocate(key);

    /// <inheritdoc/>
    public AsyncCacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKey> keys) => Internal.Allocate(keys);

    /// <inheritdoc/>
    public ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKey key, Func<CancellationToken, Task<TValue>> func) => Internal.TransferAsync(key, func);

    /// <inheritdoc/>
    public ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKey key, Func<Task<TValue>> func) => Internal.TransferAsync(key, func);

    /// <inheritdoc/>
    public ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKey key, TValue value) => Internal.TransferAsync(key, value);

    /// <inheritdoc/>
    public bool Release(TKey key) => Internal.Release(key);

    /// <inheritdoc/>
    public void Release(IEnumerable<TKey> keys) => Internal.Release(keys);
}

/// <summary>
/// 要素を保持します。
/// </summary>
/// <typeparam name="TKeyed">要素のキーを保有する型</typeparam>
/// <typeparam name="TKey">要素を選別するキーの型</typeparam>
/// <typeparam name="TValue">要素の型</typeparam>
public sealed class AsyncCacheStore<TKeyed, TKey, TValue> : IAsyncCacheStore<TKeyed, TKey, TValue>
{
    internal InternalAsyncCacheStore<TKeyed, TKey, TValue> Internal { get; }

    internal AsyncCacheStore()
    {
        Internal = new InternalAsyncCacheStore<TKeyed, TKey, TValue>();
    }

    /// <inheritdoc/>
    public AsyncCacheStore(
        Func<TKeyed, TKey> keyBuilder,
        Func<TKeyed, CancellationToken, Task<TValue>> valueBuilder,
        IDataSourceMonitoring<TKey> monitoring = default,
        int? maxCapacity = null,
        TimeSpan? timeout = null,
        CacheTimeoutBehaivor? timeoutBehaivor = CacheTimeoutBehaivor.Reset)
    {
        Internal = new InternalAsyncCacheStore<TKeyed, TKey, TValue>(
            new FuncCacheKeyFactory<TKeyed, TKey>
            {
                Build = keyBuilder
            },
            new FuncCacheValueBuilder<TKeyed, TKey, Task<TValue>>
            {
                Build = valueBuilder
            },
            monitoring,
            maxCapacity,
            timeout,
            timeoutBehaivor);
    }

    /// <inheritdoc/>
    public int MaxCapacity { get => Internal.MaxCapacity; set => Internal.MaxCapacity = value; }

    /// <inheritdoc/>
    public bool HasMaxCapacity => Internal.HasMaxCapacity;

    /// <inheritdoc/>
    public TimeSpan Timeout { get => Internal.Timeout; set => Internal.Timeout = value; }

    /// <inheritdoc/>
    public bool HasTimeout => Internal.HasTimeout;

    /// <inheritdoc/>
    public CacheTimeoutBehaivor TimeoutBehaivor { get => Internal.TimeoutBehaivor; set => Internal.TimeoutBehaivor = value; }

    /// <inheritdoc/>
    public IDataSourceMonitoring<TKey> Monitoring { get => Internal.Monitoring; internal set => Internal.Monitoring = value; }

    /// <inheritdoc/>
    public bool CanMonitoring => Internal.CanMonitoring;

    /// <inheritdoc/>
    public IEnumerable<AsyncCacheProxy<TKey, TValue>> AsEnumerable() => Internal.AsEnumerable();

    /// <inheritdoc/>
    public int Count => Internal.Count;

    /// <inheritdoc/>
    public void Clear() => Internal.Clear();

    /// <inheritdoc/>
    public void TrimExcess() => Internal.TrimExcess();

    /// <inheritdoc/>
    public Task ResetAsync() => Internal.ResetAsync();

    /// <inheritdoc/>
    public AsyncCacheProxy<TKey, TValue> Allocate(TKeyed keyed) => Internal.Allocate(keyed);

    /// <inheritdoc/>
    public AsyncCacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKeyed> keyeds) => Internal.Allocate(keyeds);

    /// <inheritdoc/>
    public ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, Func<CancellationToken, Task<TValue>> func) => Internal.TransferAsync(keyed, func);

    /// <inheritdoc/>
    public ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, Func<Task<TValue>> func) => Internal.TransferAsync(keyed, func);

    /// <inheritdoc/>
    public ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, TValue value) => Internal.TransferAsync(keyed, value);

    /// <inheritdoc/>
    public bool Release(TKeyed keyed) => Internal.Release(keyed);

    /// <inheritdoc/>
    public void Release(IEnumerable<TKeyed> keyeds) => Internal.Release(keyeds);
}

internal class InternalAsyncCacheStore<TKeyed, TKey, TValue> : IAsyncCacheStore<TKeyed, TKey, TValue>, IInternalAsyncCacheStore<TKeyed, TKey, Task<TValue>>
{
    public ICacheKeyBuilder<TKeyed, TKey> KeyBuilder { get; set; }
    public ICacheValueBuilder<TKeyed, TKey, Task<TValue>> ValueBuilder { get; set; }

    private readonly ReaderWriterLockSlim lockSlim = new(LockRecursionPolicy.SupportsRecursion);
    private readonly CacheProxyCollection<TKey, AsyncCacheProxy<TKey, TValue>> cacheProxies = new();

    internal InternalAsyncCacheStore()
    {
    }

    internal InternalAsyncCacheStore(
        ICacheKeyBuilder<TKeyed, TKey> keyBuilder,
        ICacheValueBuilder<TKeyed, TKey, Task<TValue>> valueBuilder,
        IDataSourceMonitoring<TKey> monitoring = null,
        int? maxCapacity = null,
        TimeSpan? timeout = null,
        CacheTimeoutBehaivor? timeoutBehaivor = CacheTimeoutBehaivor.Reset)
    {
        KeyBuilder = keyBuilder;
        ValueBuilder = valueBuilder;
        Monitoring = monitoring;
        if (Monitoring is not null)
        {
            Monitoring.DataSourceChanged += OnDataSourceChanged;
        }
        MaxCapacity = maxCapacity ?? 0;
        Timeout = timeout ?? TimeSpan.Zero;
        TimeoutBehaivor = timeoutBehaivor ?? CacheTimeoutBehaivor.Reset;
    }

    ~InternalAsyncCacheStore()
    {
        if (CanMonitoring && Monitoring.Running) { Monitoring.Stop(); }
        lockSlim.Dispose();
    }

    private AsyncCacheProxy<TKey, TValue> Add(TKey key, Func<CancellationToken, Task<CacheValue<TValue>>> getValueCallbackAsync)
    {
        if (HasMaxCapacity && cacheProxies.Count >= MaxCapacity)
        {
            // 最大容量を超えるときは、最初の要素を削除します。
            cacheProxies.RemoveAt(0);
        }

        AsyncCacheProxy<TKey, TValue> cacheProxy = new()
        {
            Key = key,
            Timeout = Timeout,
            TimeoutBehaivor = TimeoutBehaivor,
            GetValueCallBackAsync = getValueCallbackAsync,
            InStockCallBack = () => cacheProxies.Contains(key),
        };

        switch (TimeoutBehaivor)
        {
            case CacheTimeoutBehaivor.Release:
                cacheProxy.TimedOutCallBackAsync = () => Task.FromResult(Release(key));
                break;
            case CacheTimeoutBehaivor.Reset:
                cacheProxy.TimedOutCallBackAsync = async () =>
                {
                    await cacheProxy.ResetAsync();
                    return cacheProxy.Status is CacheStatus.Virtual;
                };
                break;
            case CacheTimeoutBehaivor.None:
            default:
                break;
        }

        cacheProxies.Add(cacheProxy);
        return cacheProxy;
    }

    public AsyncCacheProxy<TKey, TValue> Allocate(TKeyed keyed)
    {
        if (keyed is null) { throw new ArgumentNullException(nameof(keyed)); }

        using (lockSlim.Scope(LockStatus.UpgradeableRead))
        {
            var key = KeyBuilder.GetKey(keyed);
            if (cacheProxies.Contains(key)) { return cacheProxies[key]; }

            using (lockSlim.Scope(LockStatus.Write))
            {
                var proxy = Add(key, async token => new CacheValue<TValue>
                {
                    Value = await ValueBuilder.GetValue(keyed, token),
                    CachedAt = DateTimeOffset.Now,
                });
                return proxy;
            }
        }
    }

    public AsyncCacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKeyed> keyeds)
    {
        return keyeds is null
            ? Enumerable.Empty<AsyncCacheProxy<TKey, TValue>>().ToArray()
            : keyeds.Select(x => Allocate(x)).ToArray();
    }

    public async ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, Func<CancellationToken, Task<TValue>> func)
    {
        if (keyed is null) { throw new ArgumentNullException(nameof(keyed)); }

        using (lockSlim.Scope(LockStatus.UpgradeableRead))
        {
            var key = KeyBuilder.GetKey(keyed);
            if (cacheProxies.TryGetValue(key, out var cacheProxy))
            {
                await cacheProxy!.TransferAsync(() =>
                {
                    cacheProxy.GetValueCallBackAsync = async token => new CacheValue<TValue>()
                    {
                        Value = await func(token),
                        CachedAt = DateTimeOffset.Now,
                    };
                });
            }
            else
            {
                using (lockSlim.Scope(LockStatus.Write))
                {
                    cacheProxy = Add(key, async token => new CacheValue<TValue>()
                    {
                        Value = await func(token),
                        CachedAt = DateTimeOffset.Now,
                    });
                }
            }

            return cacheProxy;
        }
    }

    public ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, Func<Task<TValue>> func) => TransferAsync(keyed, _ => func());

    public ValueTask<AsyncCacheProxy<TKey, TValue>> TransferAsync(TKeyed keyed, TValue value) => TransferAsync(keyed, _ => Task.FromResult(value));

    public void Clear()
    {
        using (lockSlim.Scope(LockStatus.Write))
        {
            cacheProxies.Clear();
        }
    }

    private async Task ResetAsync(IEnumerable<TKey> keys)
    {
        if (keys is null) { return; }

        using (lockSlim.Scope(LockStatus.Read))
        {
            foreach (var cache in cacheProxies.Join(keys, x => x.Key, y => y, (x, y) => x))
            {
                await cache.ResetAsync();
            }
        }
    }

    public async Task ResetAsync()
    {
        using (lockSlim.Scope(LockStatus.Read))
        {
            foreach (var cache in cacheProxies)
            {
                await cache.ResetAsync();
            }
        }
    }

    private bool Release(TKey key)
    {
        if (key is null) { return false; }

        using (lockSlim.Scope(LockStatus.Write))
        {
            return cacheProxies.Remove(key);
        }
    }

    private void Release(IEnumerable<TKey> keys)
    {
        if (keys is null) { return; }

        using (lockSlim.Scope(LockStatus.Write))
        {
            foreach (var key in keys)
            {
                cacheProxies.Remove(key);
            }
        }
    }

    public bool Release(TKeyed keyed) => Release(KeyBuilder.GetKey(keyed));

    public void Release(IEnumerable<TKeyed> keyeds) => Release(keyeds.Select(x => KeyBuilder.GetKey(x)));

    public void TrimExcess()
    {
        using (lockSlim.Scope(LockStatus.Write))
        {
            var items = cacheProxies.Where(x => x.Status is CacheStatus.Real && !x.TimedOut).ToArray();
            cacheProxies.Clear();
            foreach (var item in items)
            {
                cacheProxies.Add(item);
            }
        }
    }

    public void OnDataSourceChanged(object sender, DataSourceChangedEventArgs<TKey> e)
    {
        switch (e.RefreshWith)
        {
            case RefreshCacheWith.Reset:
                _ = ResetAsync();
                break;

            case RefreshCacheWith.ResetContains:
                if (e.Keys?.Length > 0)
                {
                    _ = ResetAsync(e.Keys);
                }
                break;

            case RefreshCacheWith.Clear:
                Clear();
                break;

            case RefreshCacheWith.Release:
                if (e.Keys?.Length > 0)
                {
                    Release(e.Keys);
                }
                break;

            case RefreshCacheWith.None:
            default:
                break;
        }
    }

    public IEnumerable<AsyncCacheProxy<TKey, TValue>> AsEnumerable()
    {
        using (lockSlim.Scope(LockStatus.Read))
        {
            return cacheProxies.ToArray();
        }
    }

    public int Count => cacheProxies.Count;
    public IDataSourceMonitoring<TKey> Monitoring { get; set; }
    public bool CanMonitoring => Monitoring is not null;
    public bool HasMaxCapacity => MaxCapacity > 0;
    public bool HasTimeout => Timeout > TimeSpan.Zero;
    public TimeSpan Timeout { get; set; } = TimeSpan.Zero;
    public int MaxCapacity { get; set; } = 0;
    public CacheTimeoutBehaivor TimeoutBehaivor { get; set; } = CacheTimeoutBehaivor.Reset;
}
