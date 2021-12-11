namespace MSyics.Cacheyi.Configuration;

/// <summary>
/// CacheStore の設定値を表します。
/// </summary>
public interface ICacheStoreSettings
{
    /// <summary>
    /// 保持できる最大容量を取得または設定します。
    /// </summary>
    int? MaxCapacity { get; set; }

    /// <summary>
    /// 保持期間を取得または設定します。
    /// </summary>
    TimeSpan? Timeout { get; set; }

    /// <summary>
    /// 保持期間を過ぎた際の挙動を取得または設定します。
    /// </summary>
    CacheTimeoutBehaivor? TimeoutBehavior { get; set; }
}

internal class CacheStoreSettings : ICacheStoreSettings
{
    public int? MaxCapacity { get; set; }
    public TimeSpan? Timeout { get; set; }
    public CacheTimeoutBehaivor? TimeoutBehavior { get; set; } = CacheTimeoutBehaivor.Reset;
}
