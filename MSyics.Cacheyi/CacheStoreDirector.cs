/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
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
        private CacheContext Context;

        internal CacheStoreDirector(CacheContext context)
        {
            Context = context;
        }

        /// <summary>
        /// 指定した CacheStore 型のプロパティを構築します。
        /// </summary>
        /// <typeparam name="TKey">キー</typeparam>
        /// <typeparam name="TValue">値</typeparam>
        /// <param name="property">CacheStore 型のプロパティ</param>
        public ICacheStoreConfiguration<TKey, TValue> Build<TKey, TValue>(Expression<Func<CacheStore<TKey, TValue>>> property) =>
            new CacheStoreConfiguration<TKey, TValue>(Context, ((MemberExpression)property.Body).Member.Name);

        /// <summary>
        /// 指定した CacheStore 型のプロパティを構築します。
        /// </summary>
        /// <typeparam name="TKeyed">キー</typeparam>
        /// <typeparam name="TValue">値</typeparam>
        /// <param name="property">CacheStore 型のプロパティ</param>
        public ICacheStoreConfiguration<TKey, TKeyed, TValue> Build<TKey, TKeyed, TValue>(Expression<Func<CacheStore<TKey, TKeyed, TValue>>> property) =>
            new CacheStoreConfiguration<TKey, TKeyed, TValue>(Context, ((MemberExpression)property.Body).Member.Name);
    }
}
