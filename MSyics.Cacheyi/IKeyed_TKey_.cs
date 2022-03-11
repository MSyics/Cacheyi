namespace MSyics.Cacheyi;

/// <summary>
/// キーを取得できる機能を提供します。
/// </summary>
/// <typeparam name="TKey">キーの型</typeparam>
public interface IKeyed<TKey>
{
    /// <summary>
    /// キーを取得します。
    /// </summary>
    TKey Key { get; }
}
