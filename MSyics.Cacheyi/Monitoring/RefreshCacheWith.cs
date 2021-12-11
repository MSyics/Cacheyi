namespace MSyics.Cacheyi.Monitoring;

/// <summary>
/// データソースに変更があったときの CacheStore への要求を表します。
/// </summary>
public enum RefreshCacheWith
{
    /// <summary>
    /// 何もしません。
    /// </summary>
    None,

    /// <summary>
    /// すべてリセットします。
    /// </summary>
    Reset,

    /// <summary>
    /// 変更箇所をリセットします。
    /// </summary>
    ResetContains,

    /// <summary>
    /// すべて解除します。
    /// </summary>
    Clear,

    /// <summary>
    /// 変更箇所を解除します。
    /// </summary>
    Release,
}
