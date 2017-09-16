/****************************************************************
© 2017 MSyics
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
        private CacheContext m_context;

        internal CacheStoreDirector(CacheContext context)
        {
            m_context = context;
        }

        /// <summary>
        /// 指定した CacheStore 型のプロパティを構築します。
        /// </summary>
        /// <typeparam name="TKey">キー</typeparam>
        /// <typeparam name="TValue">値</typeparam>
        /// <param name="property">CacheStore 型のプロパティ</param>
        public ICacheStoreConfiguration<TKey, TValue> Build<TKey, TValue>(Expression<Func<CacheStore<TKey, TValue>>> property) =>
            new CacheStoreConfiguration<TKey, TValue>(m_context, ((MemberExpression)property.Body).Member.Name);

        /// <summary>
        /// 指定した CacheStore 型のプロパティを構築します。
        /// </summary>
        /// <typeparam name="TKey">キー</typeparam>
        /// <typeparam name="TValue">値</typeparam>
        /// <param name="property">CacheStore 型のプロパティ</param>
        public ICacheStoreConfiguration<TUnique, TKey, TValue> Build<TUnique, TKey, TValue>(Expression<Func<CacheStore<TUnique, TKey, TValue>>> property) =>
            new CacheStoreConfiguration<TUnique, TKey, TValue>(m_context, ((MemberExpression)property.Body).Member.Name);
    }
}
