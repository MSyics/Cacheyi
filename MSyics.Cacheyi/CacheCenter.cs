﻿/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using MSyics.Cacheyi.Configuration;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュの実装を提供します。これは抽象クラスです。
    /// </summary>
    public abstract class CacheCenter
    {
        /// <summary>
        /// CacheCenter クラスのインスタンスを初期化します。
        /// </summary>
        public CacheCenter()
        {
            Context = new CacheContext() { CenterType = GetType(), };

            var initializer = new CacheContext().CenterInitializerTypedMapping.GetOrAdd(Context.CenterType, key =>
            {
                ConstructStore(new CacheStoreDirector(Context));
                return new CacheCenterInitializerFactory().Create(this);
            });
            initializer(this);
        }

        /// <summary>
        /// CacheStore の設定を行います。
        /// </summary>
        /// <param name="director">CacheStore を構築するための CacheStoreDirector オブジェクト</param>
        protected abstract void ConstructStore(CacheStoreDirector director);

        internal CacheContext Context { get; set; }
    }
}
