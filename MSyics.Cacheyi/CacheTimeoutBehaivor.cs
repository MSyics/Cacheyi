namespace MSyics.Cacheyi;

/// <summary>
/// 要素の保持期間を過ぎた際の挙動を表します。
/// </summary>
public enum CacheTimeoutBehaivor
{
    /// <summary>
    /// 何もしません。
    /// </summary>
    None,

    /// <summary>
    /// 解除します。
    /// </summary>
    Release,

    /// <summary>
    /// リセットします。
    /// </summary>
    Reset,
}
