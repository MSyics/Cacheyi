namespace MSyics.Cacheyi;

/// <summary>
/// 要素を取得するためのプロキシ
/// </summary>
/// <typeparam name="TKey">キーの型</typeparam>
/// <typeparam name="TValue">保持要素の型</typeparam>
public sealed class CacheProxy<TKey, TValue> : IKeyed<TKey>
{
    private readonly object lockObj = new();
    private readonly CacheProxyCollection<TKey, CacheProxy<TKey, TValue>> cacheProxies;

    internal CacheProxy(CacheProxyCollection<TKey, CacheProxy<TKey, TValue>> cacheProxies)
    {
        this.cacheProxies = cacheProxies;
    }

    /// <summary>
    /// 要素を取得します。
    /// </summary>
    public TValue GetValue(CancellationToken cancellationToken = default)
    {
        lock (lockObj)
        {
            if (Status is CacheStatus.Virtual)
            {
                CacheValue = GetValueCallBack(cancellationToken);
                Status = CacheStatus.Real;

                if (HasTimeout)
                {
                    CancellingTimeout = TimedOutCallBack?.StartNewTimer(Timeout);
                }
            }
            return CacheValue.Value;
        }
    }

    /// <summary>
    /// 要素の取得を試みます。
    /// </summary>
    /// <param name="value">取得した値です。取得に失敗した場合は初期値を返します。</param>
    /// <param name="cancellationToken"></param>
    /// <returns>取得に成功した場合は true、それ以外は false。</returns>
    public bool TryGetValue(out TValue value, CancellationToken cancellationToken = default)
    {
        try
        {
            value = GetValue(cancellationToken);
            return true;
        }
        catch (Exception)
        {
            value = default;
            return false;
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
    public CacheProxy<TKey, TValue> Reset()
    {
        lock (lockObj)
        {
            ResetCore();
            return this;
        }
    }

    /// <summary>
    /// 要素の保持状態を保持期間を過ぎていたらリセットします。
    /// </summary>
    public CacheProxy<TKey, TValue> ResetIfTimeout()
    {
        lock (lockObj)
        {
            if (TimedOut)
            {
                ResetCore();
            }
            return this;
        }
    }

    internal void Transfer(Func<CancellationToken, TValue> func, Action<CacheProxy<TKey, TValue>, Func<CancellationToken, TValue>> action)
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
    internal Func<CancellationToken, CacheValue<TValue>> GetValueCallBack { get; set; }
    internal Func<bool> TimedOutCallBack { get; set; }
    internal TaskCompletionSource<bool> CancellingTimeout { get; set; }
}
