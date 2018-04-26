/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/

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
            var init = Context.CenterInitializers.GetOrAdd(Context.CenterType, _ =>
            {
                ConstructStore(new CacheStoreDirector() { Context = Context });
                return new CacheCenterInitializerFactory().Create(this);
            });
            init(this);
        }

        /// <summary>
        /// CacheStore の設定を行います。
        /// </summary>
        /// <param name="director">CacheStore を構築するための機能を持つオブジェクト</param>
        protected abstract void ConstructStore(CacheStoreDirector director);

        internal CacheContext Context { get; set; }
    }
}
