namespace MSyics.Cacheyi.Configuration;

/// <summary>
/// 保持する要素を選別するキーの設定を行います。
/// </summary>
public interface ICacheKeyConfiguration<TKeyed, TKey, TValue>
{
    /// <summary>
    /// 保持する要素を選別するキーを取得します。
    /// </summary>
    /// <param name="builder">指定した要素からキーを取得する機能</param>
    ICacheValueConfiguration<TKeyed, TKey, TValue> GetKey(Func<TKeyed, TKey> builder);
}
