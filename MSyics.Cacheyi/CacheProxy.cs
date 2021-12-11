namespace MSyics.Cacheyi;

/// <summary>
/// 要素を取得するためのプロキシ
/// </summary>
/// <typeparam name="TKey">キーの型</typeparam>
/// <typeparam name="TValue">保持要素の型</typeparam>
public sealed class CacheProxy<TKey, TValue>
{
    private readonly object lockObj = new();

    internal CacheProxy() { }

    /// <summary>
    /// 要素を取得します。
    /// </summary>
    public TValue GetValue()
    {
        lock (lockObj)
        {
            if (Status is CacheStatus.Virtual)
            {
                CacheValue = GetValueCallBack();
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
    /// <returns>取得に成功した場合は true、それ以外は false。</returns>
    public bool TryGetValue(out TValue value)
    {
        value = default;
        try
        {
            value = GetValue();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 要素の保持状態をリセットします。
    /// </summary>
    public CacheProxy<TKey, TValue> Reset()
    {
        lock (lockObj)
        {
            Status = CacheStatus.Virtual;
            CancellingTimeout?.TrySetCanceled();
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
                Status = CacheStatus.Virtual;
                CancellingTimeout?.TrySetCanceled();
            }
            return this;
        }
    }

    internal void Transfer(Action<CacheProxy<TKey, TValue>> action)
    {
        lock (lockObj)
        {
            action?.Invoke(this);
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
    internal Func<CacheValue<TValue>> GetValueCallBack { get; set; }
    internal Func<bool> TimedOutCallBack { get; set; }
    internal TaskCompletionSource<bool> CancellingTimeout { get; set; }
    internal Func<bool> InStockCallBack { get; set; }
}
