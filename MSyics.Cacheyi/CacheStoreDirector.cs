using MSyics.Cacheyi.Configuration;
using System.Linq.Expressions;

namespace MSyics.Cacheyi;

/// <summary>
/// キャッシュの実装を構築するための機能を提供します。
/// </summary>
public sealed class CacheStoreDirector
{
    internal CacheStoreDirector() { }

    private static string GetStoreName(MemberExpression exp) => $"{exp.Member.ReflectedType.FullName}.{exp.Member.Name}";

    /// <summary>
    /// 指定した CacheStore 型のプロパティを構築します。   
    /// </summary>
    /// <typeparam name="TKey">キーの型</typeparam>
    /// <typeparam name="TValue">要素の型</typeparam>
    /// <param name="property">CacheStore 型のプロパティ</param>
    public static ICacheStoreConfiguration<TKey, TValue> Build<TKey, TValue>(Expression<Func<ICacheStore<TKey, TValue>>> property) =>
        new CacheStoreConfiguration<TKey, TValue>(GetStoreName((MemberExpression)property.Body));

    /// <summary>
    /// 指定した CacheStore 型のプロパティを構築します。
    /// </summary>
    /// <typeparam name="TKeyed">キーの型</typeparam>
    /// <typeparam name="TKey">キーの型</typeparam>
    /// <typeparam name="TValue">要素の型</typeparam>
    /// <param name="property">CacheStore 型のプロパティ</param>
    public static ICacheStoreConfiguration<TKeyed, TKey, TValue> Build<TKeyed, TKey, TValue>(Expression<Func<ICacheStore<TKeyed, TKey, TValue>>> property) =>
        new CacheStoreConfiguration<TKeyed, TKey, TValue>(GetStoreName((MemberExpression)property.Body));
}

/// <summary/>
public static class CacheStoreDirectorExtensions
{
    /// <summary>
    /// 指定した CacheStore 型のプロパティを構築します。   
    /// </summary>
    /// <typeparam name="TKey">キーの型</typeparam>
    /// <typeparam name="TValue">要素の型</typeparam>
    /// <param name="_"></param>
    /// <param name="property">CacheStore 型のプロパティ</param>
    public static ICacheStoreConfiguration<TKey, TValue> Build<TKey, TValue>(this CacheStoreDirector _, Expression<Func<ICacheStore<TKey, TValue>>> property) =>
        CacheStoreDirector.Build(property);

    /// <summary>
    /// 指定した CacheStore 型のプロパティを構築します。
    /// </summary>
    /// <typeparam name="TKeyed">キーの型</typeparam>
    /// <typeparam name="TKey">キーの型</typeparam>
    /// <typeparam name="TValue">要素の型</typeparam>
    /// <param name="_"></param>
    /// <param name="property">CacheStore 型のプロパティ</param>
    public static ICacheStoreConfiguration<TKeyed, TKey, TValue> Build<TKeyed, TKey, TValue>(this CacheStoreDirector _, Expression<Func<ICacheStore<TKeyed, TKey, TValue>>> property) =>
        CacheStoreDirector.Build(property);
}