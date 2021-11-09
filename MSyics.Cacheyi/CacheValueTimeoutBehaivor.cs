namespace MSyics.Cacheyi;

/// <summary>
/// 保持期間を過ぎた際の挙動を表します。
/// </summary>
public enum CacheValueTimeoutBehaivor
{
    /// <summary>
    /// 何もしません。
    /// </summary>
    None,

    /// <summary>
    /// 削除します。
    /// </summary>
    Remove,

    /// <summary>
    /// リセットします。
    /// </summary>
    Reset,
}
