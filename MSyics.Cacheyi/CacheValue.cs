namespace MSyics.Cacheyi;

/// <summary>
/// 保持する要素を表します。
/// </summary>
internal record struct CacheValue<TValue>(TValue Value, DateTimeOffset CachedAt);
