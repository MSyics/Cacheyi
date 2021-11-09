namespace MSyics.Cacheyi;

internal interface ICacheValueBuilder<TKeyed, TKey, TValue>
{
    TValue GetValue(TKeyed keyed);
}

internal sealed class FuncCacheValueBuilder<TKeyed, TKey, TValue> : ICacheValueBuilder<TKeyed, TKey, TValue>
{
    public Func<TKeyed, TValue> Build { get; set; }
    public TValue GetValue(TKeyed keyed) => Build(keyed);
}
