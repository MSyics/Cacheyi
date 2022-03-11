namespace MSyics.Cacheyi;

internal interface ICacheValueBuilder<TKeyed, TKey, TValue>
{
    TValue GetValue(TKeyed keyed, CancellationToken cancellationToken);
}

internal sealed class FuncCacheValueBuilder<TKeyed, TKey, TValue> : ICacheValueBuilder<TKeyed, TKey, TValue>
{
    public Func<TKeyed, CancellationToken, TValue> Build { get; set; }
    public TValue GetValue(TKeyed keyed, CancellationToken cancellationToken) => Build(keyed, cancellationToken);
}
