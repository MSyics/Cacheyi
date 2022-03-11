namespace MSyics.Cacheyi;

/// <summary>
/// 要素を取得するためのプロキシ
/// </summary>
/// <typeparam name="TKey">キーの型</typeparam>
/// <typeparam name="TValue">保持要素の型</typeparam>
public sealed class AsyncCacheProxy<TKey, TValue> : IKeyed<TKey>
{
    private readonly AsyncLock lockObj = new();

    internal AsyncCacheProxy() { }

    /// <summary>
    /// 要素を取得します。
    /// </summary>
    public async ValueTask<TValue> GetValueAsync(CancellationToken cancellationToken = default)
    {
        using (await lockObj.LockAsync(cancellationToken))
        {
            if (Status is CacheStatus.Virtual)
            {
                CacheValue = await GetValueCallBackAsync(cancellationToken).ConfigureAwait(false);
                Status = CacheStatus.Real;

                if (HasTimeout)
                {
                    CancellingTimeout = TimedOutCallBackAsync?.StartNewTimer(Timeout);
                }
            }

            return CacheValue.Value;
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
    public async Task ResetAsync()
    {
        using (await lockObj.LockAsync())
        {
            ResetCore();
        }
    }

    /// <summary>
    /// 要素の保持状態を保持期間を過ぎていたらリセットします。
    /// </summary>
    public async Task ResetIfTimeoutAsync()
    {
        using (await lockObj.LockAsync())
        {
            if (TimedOut)
            {
                ResetCore();
            }
        }
    }

    internal async Task TransferAsync(Action action)
    {
        using (await lockObj.LockAsync())
        {
            ResetCore();
            action?.Invoke();
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
            if (Status != CacheStatus.Real) { return false; }
            return HasTimeout && DateTimeOffset.Now >= CacheValue.CachedAt.Add(Timeout);
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
    public bool InStock => InStockCallBack?.Invoke() ?? false;

    internal CacheValue<TValue> CacheValue { get; set; }
    internal Func<CancellationToken, Task<CacheValue<TValue>>> GetValueCallBackAsync { get; set; }
    internal Func<Task<bool>> TimedOutCallBackAsync { get; set; }
    internal TaskCompletionSource<Task<bool>> CancellingTimeout { get; set; }
    internal Func<bool> InStockCallBack { get; set; }
}
