using MSyics.Cacheyi.Configuration;
using System;
using System.Linq.Expressions;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュの実装を構築するための機能を提供します。
    /// </summary>
    public sealed class CacheStoreDirector
    {
        internal CacheStoreDirector() { }
        private string GetStoreName(MemberExpression exp) => $"{exp.Member.ReflectedType.FullName}.{exp.Member.Name}";

        /// <summary>
        /// 指定した CacheStore 型のプロパティを構築します。   
        /// </summary>
        /// <typeparam name="TKey">キーの型</typeparam>
        /// <typeparam name="TValue">要素の型</typeparam>
        /// <param name="property">CacheStore 型のプロパティ</param>
        public ICacheStoreConfiguration<TKey, TValue> Build<TKey, TValue>(Expression<Func<CacheStore<TKey, TValue>>> property) =>
            new CacheStoreConfiguration<TKey, TValue>(GetStoreName((MemberExpression)property.Body));

        /// <summary>
        /// 指定した CacheStore 型のプロパティを構築します。
        /// </summary>
        /// <typeparam name="TKeyed">キーの型</typeparam>
        /// <typeparam name="TKey">キーの型</typeparam>
        /// <typeparam name="TValue">要素の型</typeparam>
        /// <param name="property">CacheStore 型のプロパティ</param>
        public ICacheStoreConfiguration<TKeyed, TKey, TValue> Build<TKeyed, TKey, TValue>(Expression<Func<CacheStore<TKeyed, TKey, TValue>>> property) =>
            new CacheStoreConfiguration<TKeyed, TKey, TValue>(GetStoreName((MemberExpression)property.Body));
    }
}
