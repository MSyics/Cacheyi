namespace MSyics.Cacheyi;

/// <summary>
/// 保持する要素を表します。
/// </summary>
public sealed class CacheValue<TValue>
{
    internal CacheValue() { }

    /// <summary>
    /// 要素を取得します。
    /// </summary>
    public TValue Value { get; internal set; }

    /// <summary>
    /// 保持した日時を取得します。
    /// </summary>
    public DateTimeOffset CachedAt { get; internal set; }
}
