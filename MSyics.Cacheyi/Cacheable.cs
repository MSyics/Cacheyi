using MSyics.Cacheyi.Configuration;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace MSyics.Cacheyi
{
    public class Cacheable
    {
        /// <summary>
        /// CacheStore の設定を行います。
        /// </summary>
        /// <param name="director">CacheStore を構築するための機能を持つオブジェクト</param>
        public static CacheStore<TKey, TValue> Setup<TKey, TValue>(Expression<Func<CacheStore<TKey, TValue>>> property, Action<ICacheStoreConfiguration<TKey, TValue>> cnstruct)
        {
            cnstruct(new CacheStoreDirector().Build(property));
            return property.Compile()();
        }
    }
}
