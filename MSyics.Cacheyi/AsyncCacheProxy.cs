namespace MSyics.Cacheyi;

/// <summary>
/// 要素を取得するためのプロキシ
/// </summary>
/// <typeparam name="TKey">キーの型</typeparam>
/// <typeparam name="TValue">保持要素の型</typeparam>
public sealed class AsyncCacheProxy<TKey, TValue> : IKeyed<TKey>
{
    private readonly object lockObj = new();
    private readonly AsyncLock asyncLock = new();
    private readonly CacheProxyCollection<TKey, AsyncCacheProxy<TKey, TValue>> cacheProxies;

    internal AsyncCacheProxy(CacheProxyCollection<TKey, AsyncCacheProxy<TKey, TValue>> cacheProxies)
    {
        this.cacheProxies = cacheProxies;
    }

    /// <summary>
    /// 要素を取得します。
    /// </summary>
    public async ValueTask<TValue> GetValueAsync(CancellationToken cancellationToken = default)
    {
        CacheValue<TValue> value;

        using (await asyncLock.LockAsync(cancellationToken))
        {
            if (Status is CacheStatus.Real)
            {
                return CacheValue.Value;
            }

            value = await GetValueCallBackAsync(cancellationToken).ConfigureAwait(false);

            lock (lockObj)
            {
                Status = CacheStatus.Real;
                CacheValue = value;
                if (HasTimeout)
                {
                    CancellingTimeout = TimedOutCallBackAsync?.StartNewTimer(Timeout, this);
                }
                return CacheValue.Value;
            }
        }
    }

    /// <summary>
    /// 要素の取得を試みます。
    /// </summary>
    /// <returns>取得に成功した場合は true、それ以外は false。</returns>
    public async ValueTask<(bool result, TValue value)> TryGetValueAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return (true, await GetValueAsync(cancellationToken));
        }
        catch (Exception)
        {
            return default;
        }
    }

    private void ResetCore()
    {
        Status = CacheStatus.Virtual;
        CancellingTimeout?.TrySetCanceled();
    }

    /// <summary>
    /// 要素の保持状態をリセットします。
    /// </summary>
    public void Reset()
    {
        lock (lockObj)
        {
            ResetCore();
        }
    }

    /// <summary>
    /// 要素の保持状態を保持期間を過ぎていたらリセットします。
    /// </summary>
    public void ResetIfTimeout()
    {
        lock (lockObj)
        {
            if (TimedOut)
            {
                ResetCore();
            }
        }
    }

    internal void Transfer(Func<CancellationToken, Task<TValue>> func, Action<AsyncCacheProxy<TKey, TValue>, Func<CancellationToken, Task<TValue>>> action)
    {
        lock (lockObj)
        {
            ResetCore();
            action?.Invoke(this, func);
        }
    }

    /// <summary>
    /// 要素の保持状態を取得します。
    /// </summary>
    public CacheStatus Status { get; internal set; } = CacheStatus.Virtual;

    /// <summary>
    /// 要素の保持期間を経過したかどうか示す値を取得します。
    /// </summary>
    public bool TimedOut
    {
        get
        {
            lock (lockObj)
            {
                if (Status != CacheStatus.Real) { return false; }
                DateTimeOffset offset = CacheValue.CachedAt.Add(Timeout);
                return HasTimeout && DateTimeOffset.Now >= offset;
            }
        }
    }

    /// <summary>
    /// 要素の保持期間を取得します。
    /// </summary>
    public TimeSpan Timeout { get; internal set; } = TimeSpan.Zero;

    /// <summary>
    /// 要素の保持期間を過ぎた後の挙動を取得します。
    /// </summary>
    public CacheTimeoutBehaivor TimeoutBehaivor { get; internal set; } = CacheTimeoutBehaivor.Reset;

    /// <summary>
    /// 要素の保持期間を持つかどうかを示す値を取得します。
    /// </summary>
    public bool HasTimeout => Timeout > TimeSpan.Zero;

    /// <summary>
    /// 要素のキーを取得します。
    /// </summary>
    public TKey Key { get; internal set; }

    /// <summary>
    /// Store に存在するかどうかを示す値を取得します。存在する場合は true、それ以外は false を返します。
    /// </summary>
    public bool InStock => cacheProxies.Contains(Key);

    internal CacheValue<TValue> CacheValue { get; set; }
    internal Func<CancellationToken, Task<CacheValue<TValue>>> GetValueCallBackAsync { get; set; }
    internal Func<AsyncCacheProxy<TKey, TValue>, bool> TimedOutCallBackAsync { get; set; }
    internal TaskCompletionSource<bool> CancellingTimeout { get; set; }
}
