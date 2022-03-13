using MSyics.Cacheyi.Monitoring;

namespace MSyics.Cacheyi;

/// <summary>
/// 要素を保持します。
/// </summary>
/// <typeparam name="TKeyed">要素のキーを保有する型</typeparam>
/// <typeparam name="TKey">要素を選別するキーの型</typeparam>
/// <typeparam name="TValue">要素の型</typeparam>
public interface ICacheStore<TKeyed, TKey, TValue>
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
    IEnumerable<CacheProxy<TKey, TValue>> AsEnumerable();

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
    void Reset();

    /// <summary>
    /// 指定したオブジェクトから要素を引き当てます。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    CacheProxy<TKey, TValue> Allocate(TKeyed keyed);

    /// <summary>
    /// 指定したオブジェクトの一覧から要素を引き当てます。
    /// </summary>
    /// <param name="keyeds">キーを保有するオブジェクトの一覧</param>
    CacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKeyed> keyeds);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    /// <param name="func">振替値を取得する処理</param>
    CacheProxy<TKey, TValue> Transfer(TKeyed keyed, Func<CancellationToken, TValue> func);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    /// <param name="func">振替値を取得する処理</param>
    CacheProxy<TKey, TValue> Transfer(TKeyed keyed, Func<TValue> func);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="keyed">キーを保有するオブジェクト</param>
    /// <param name="value">振替値</param>
    CacheProxy<TKey, TValue> Transfer(TKeyed keyed, TValue value);

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
public interface ICacheStore<TKey, TValue> : ICacheStore<TKey, TKey, TValue>
{
    /// <summary>
    /// 指定したキーから要素を引き当てます。
    /// </summary>
    /// <param name="key">キー</param>
    new CacheProxy<TKey, TValue> Allocate(TKey key);

    /// <summary>
    /// 指定したキーの一覧から要素を引き当てます。
    /// </summary>
    /// <param name="keys">キーの一覧</param>
    new CacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKey> keys);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="func">振替値を取得する処理</param>
    new CacheProxy<TKey, TValue> Transfer(TKey key, Func<CancellationToken, TValue> func);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="func">振替値を取得する処理</param>
    new CacheProxy<TKey, TValue> Transfer(TKey key, Func<TValue> func);

    /// <summary>
    /// 引当要素の値を振り替えます。
    /// </summary>
    /// <param name="key">キー</param>
    /// <param name="value">振替値</param>
    new CacheProxy<TKey, TValue> Transfer(TKey key, TValue value);

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
public sealed class CacheStore<TKey, TValue> : ICacheStore<TKey, TValue>
{
    internal InternalCacheStore<TKey, TKey, TValue> Internal { get; }

    internal CacheStore()
    {
        Internal = new InternalCacheStore<TKey, TKey, TValue>
        {
            KeyBuilder = new FuncCacheKeyFactory<TKey, TKey>() { Build = key => key }
        };
    }

    /// <inheritdoc/>
    public CacheStore(
        Func<TKey, CancellationToken, TValue> valueBuilder,
        IDataSourceMonitoring<TKey> monitoring = default,
        int? maxCapacity = null,
        TimeSpan? timeout = null,
        CacheTimeoutBehaivor? timeoutBehaivor = CacheTimeoutBehaivor.Reset)
    {
        Internal = new InternalCacheStore<TKey, TKey, TValue>(
            new FuncCacheKeyFactory<TKey, TKey>
            {
                Build = key => key
            },
            new FuncCacheValueBuilder<TKey, TKey, TValue>
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
    public IEnumerable<CacheProxy<TKey, TValue>> AsEnumerable() => Internal.AsEnumerable();

    /// <inheritdoc/>
    public int Count => Internal.Count;

    /// <inheritdoc/>
    public void Clear() => Internal.Clear();

    /// <inheritdoc/>
    public void TrimExcess() => Internal.TrimExcess();

    /// <inheritdoc/>
    public void Reset() => Internal.Reset();

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue> Allocate(TKey key) => Internal.Allocate(key);

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKey> keys) => Internal.Allocate(keys);

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue> Transfer(TKey key, Func<CancellationToken, TValue> func) => Internal.Transfer(key, func);

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue> Transfer(TKey key, Func<TValue> func) => Internal.Transfer(key, func);

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue> Transfer(TKey key, TValue value) => Internal.Transfer(key, value);

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
public sealed class CacheStore<TKeyed, TKey, TValue> : ICacheStore<TKeyed, TKey, TValue>
{
    internal InternalCacheStore<TKeyed, TKey, TValue> Internal { get; }

    internal CacheStore()
    {
        Internal = new InternalCacheStore<TKeyed, TKey, TValue>();
    }

    /// <inheritdoc/>
    public CacheStore(
        Func<TKeyed, TKey> keyBuilder,
        Func<TKeyed, CancellationToken, TValue> valueBuilder,
        IDataSourceMonitoring<TKey> monitoring = default,
        int? maxCapacity = null,
        TimeSpan? timeout = null,
        CacheTimeoutBehaivor? timeoutBehaivor = CacheTimeoutBehaivor.Reset)
    {
        Internal = new InternalCacheStore<TKeyed, TKey, TValue>(
            new FuncCacheKeyFactory<TKeyed, TKey>
            {
                Build = keyBuilder
            },
            new FuncCacheValueBuilder<TKeyed, TKey, TValue>
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
    public IEnumerable<CacheProxy<TKey, TValue>> AsEnumerable() => Internal.AsEnumerable();

    /// <inheritdoc/>
    public int Count => Internal.Count;

    /// <inheritdoc/>
    public void Clear() => Internal.Clear();

    /// <inheritdoc/>
    public void TrimExcess() => Internal.TrimExcess();

    /// <inheritdoc/>
    public void Reset() => Internal.Reset();

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue> Allocate(TKeyed keyed) => Internal.Allocate(keyed);

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKeyed> keyeds) => Internal.Allocate(keyeds);

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue> Transfer(TKeyed keyed, TValue value) => Internal.Transfer(keyed, value);

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue> Transfer(TKeyed keyed, Func<CancellationToken, TValue> func) => Internal.Transfer(keyed, func);

    /// <inheritdoc/>
    public CacheProxy<TKey, TValue> Transfer(TKeyed keyed, Func<TValue> func) => Internal.Transfer(keyed, func);

    /// <inheritdoc/>
    public bool Release(TKeyed keyed) => Internal.Release(keyed);

    /// <inheritdoc/>
    public void Release(IEnumerable<TKeyed> keyeds) => Internal.Release(keyeds);
}

internal interface IInternalAsyncCacheStore<TKeyed, TKey, TValue>
{
    void OnDataSourceChanged(object sender, DataSourceChangedEventArgs<TKey> e);

    ICacheKeyBuilder<TKeyed, TKey> KeyBuilder { get; set; }
    ICacheValueBuilder<TKeyed, TKey, TValue> ValueBuilder { get; set; }
    int MaxCapacity { get; set; }
    IDataSourceMonitoring<TKey> Monitoring { get; set; }
    TimeSpan Timeout { get; set; }
    CacheTimeoutBehaivor TimeoutBehaivor { get; set; }
}

internal class InternalCacheStore<TKeyed, TKey, TValue> : ICacheStore<TKeyed, TKey, TValue>, IInternalAsyncCacheStore<TKeyed, TKey, TValue>
{
    public ICacheKeyBuilder<TKeyed, TKey> KeyBuilder { get; set; }
    public ICacheValueBuilder<TKeyed, TKey, TValue> ValueBuilder { get; set; }

    private readonly ReaderWriterLockSlim lockSlim = new();
    private readonly CacheProxyCollection<TKey, CacheProxy<TKey, TValue>> cacheProxies = new();

    internal InternalCacheStore()
    {
    }

    internal InternalCacheStore(
        ICacheKeyBuilder<TKeyed, TKey> keyBuilder,
        ICacheValueBuilder<TKeyed, TKey, TValue> valueBuilder,
        IDataSourceMonitoring<TKey> monitoring = default,
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

    ~InternalCacheStore()
    {
        if (CanMonitoring && Monitoring.Running) { Monitoring.Stop(); }
        lockSlim.Dispose();
    }

    private CacheProxy<TKey, TValue> Add(TKey key, Func<CancellationToken, CacheValue<TValue>> getValueCallback)
    {
        if (HasMaxCapacity && cacheProxies.Count >= MaxCapacity)
        {
            // 最大容量を超えるときは、最初の要素を削除します。
            cacheProxies.RemoveAt(0);
        }

        CacheProxy<TKey, TValue> cacheProxy = new(cacheProxies)
        {
            Key = key,
            Timeout = Timeout,
            TimeoutBehaivor = TimeoutBehaivor,
            GetValueCallBack = getValueCallback,
        };

        switch (TimeoutBehaivor)
        {
            case CacheTimeoutBehaivor.Release:
                cacheProxy.TimedOutCallBack = () => Release(key);
                break;
            case CacheTimeoutBehaivor.Reset:
                cacheProxy.TimedOutCallBack = () => cacheProxy.Reset().Status is CacheStatus.Virtual;
                break;
            case CacheTimeoutBehaivor.None:
            default:
                break;
        }

        cacheProxies.Add(cacheProxy);
        return cacheProxy;
    }

    public CacheProxy<TKey, TValue> Allocate(TKeyed keyed)
    {
        if (keyed is null) { throw new ArgumentNullException(nameof(keyed)); }

        var key = KeyBuilder.GetKey(keyed);
        using (lockSlim.Scope(LockStatus.UpgradeableRead))
        {
            if (cacheProxies.Contains(key)) { return cacheProxies[key]; }

            using (lockSlim.Scope(LockStatus.Write))
            {
                return Add(key, token => new CacheValue<TValue>(ValueBuilder.GetValue(keyed, token), DateTimeOffset.Now));
            }
        }
    }

    public CacheProxy<TKey, TValue>[] Allocate(IEnumerable<TKeyed> keyeds)
    {
        return keyeds is null
            ? Enumerable.Empty<CacheProxy<TKey, TValue>>().ToArray()
            : keyeds.Select(x => Allocate(x)).ToArray();
    }

    public CacheProxy<TKey, TValue> Transfer(TKeyed keyed, Func<CancellationToken, TValue> func)
    {
        if (keyed is null) { throw new ArgumentNullException(nameof(keyed)); }

        var key = KeyBuilder.GetKey(keyed);
        using (lockSlim.Scope(LockStatus.UpgradeableRead))
        {
            if (cacheProxies.TryGetValue(key, out var cacheProxy))
            {
                cacheProxy!.Transfer(func, static (proxy, value) =>
                {
                    proxy.GetValueCallBack = token => new CacheValue<TValue>()
                    {
                        Value = value(token),
                        CachedAt = DateTimeOffset.Now,
                    };
                });
            }
            else
            {
                using (lockSlim.Scope(LockStatus.Write))
                {
                    cacheProxy = Add(key, token => new CacheValue<TValue>()
                    {
                        Value = func(token),
                        CachedAt = DateTimeOffset.Now,
                    });
                }
            }

            return cacheProxy;
        }
    }

    public CacheProxy<TKey, TValue> Transfer(TKeyed keyed, Func<TValue> func) => Transfer(keyed, _ => func());

    public CacheProxy<TKey, TValue> Transfer(TKeyed keyed, TValue value) => Transfer(keyed, _ => value);

    public void Clear()
    {
        using (lockSlim.Scope(LockStatus.Write))
        {
            cacheProxies.Clear();
        }
    }

    private void Reset(IEnumerable<TKey> keys)
    {
        if (keys is null) { return; }

        using (lockSlim.Scope(LockStatus.Read))
        {
            foreach (var cache in cacheProxies.Join(keys, x => x.Key, y => y, (x, y) => x))
            {
                cache.Reset();
            }
        }
    }

    public void Reset()
    {
        using (lockSlim.Scope(LockStatus.Read))
        {
            foreach (var cache in cacheProxies)
            {
                cache.Reset();
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
            foreach (var key in keys.ToArray())
            {
                cacheProxies.Remove(key);
            }
        }
    }

    public bool Release(TKeyed keyed) => Release(KeyBuilder.GetKey(keyed));

    public void Release(IEnumerable<TKeyed> keyeds) => Release(keyeds.Select(x => KeyBuilder.GetKey(x)));

    public void TrimExcess()
    {
        using (lockSlim.Scope(LockStatus.UpgradeableRead))
        {
            var items = cacheProxies.Where(x => x.Status is CacheStatus.Real && !x.TimedOut).ToArray();

            using (lockSlim.Scope(LockStatus.Write))
            {
                cacheProxies.Clear();
                foreach (var item in items)
                {
                    cacheProxies.Add(item);
                }
            }
        }
    }

    public void OnDataSourceChanged(object sender, DataSourceChangedEventArgs<TKey> e)
    {
        switch (e.RefreshWith)
        {
            case RefreshCacheWith.Reset:
                Reset();
                break;

            case RefreshCacheWith.ResetContains:
                if (e.Keys?.Length > 0)
                {
                    Reset(e.Keys);
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

    public IEnumerable<CacheProxy<TKey, TValue>> AsEnumerable()
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
