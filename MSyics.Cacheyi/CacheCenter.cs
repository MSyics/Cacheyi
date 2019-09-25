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
            var init = new CacheContext().CenterInitializers.GetOrAdd(GetType(), type =>
            {
                ConstructStore(new CacheStoreDirector());
                return new CacheCenterInitializerFactory().Create(type);
            });
            init(this);
        }

        /// <summary>
        /// CacheStore の設定を行います。
        /// </summary>
        /// <param name="director">CacheStore を構築するための機能を持つオブジェクト</param>
        protected abstract void ConstructStore(CacheStoreDirector director);
    }
}
