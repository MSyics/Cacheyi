using System;
using System.Collections.Concurrent;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュの実装を提供します。これは抽象クラスです。
    /// </summary>
    public abstract class CacheCenter
    {
        internal static readonly ConcurrentDictionary<Type, Action<object>> centerInitializers = new ConcurrentDictionary<Type, Action<object>>();
        internal static readonly CacheStoreCollection stores = new CacheStoreCollection();

        /// <summary>
        /// CacheCenter クラスのインスタンスを初期化します。
        /// </summary>
        public CacheCenter() => ConstructStore(this, ConstructStore);

        /// <summary>
        /// CacheStore の設定を行います。
        /// </summary>
        /// <param name="director">CacheStore を構築するための機能を持つオブジェクト</param>
        protected abstract void ConstructStore(CacheStoreDirector director);

        /// <summary>
        /// CacheStore の設定を行います。
        /// </summary>
        /// <param name="obj">CacheStore を持つオブジェクト</param>
        /// <param name="director">CacheStore を構築する処理</param>
        public static void ConstructStore(object obj, Action<CacheStoreDirector> director)
        {
            var init = centerInitializers.GetOrAdd(obj.GetType(), type =>
            {
                director?.Invoke(new CacheStoreDirector());
                return new CacheCenterInitializerFactory().Create(type);
            });
            init(obj);
        }
    }
}
