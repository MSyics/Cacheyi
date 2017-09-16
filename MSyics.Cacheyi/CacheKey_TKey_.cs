/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュするときに使用するキーを表現します。
    /// </summary>
    internal sealed class CacheKey<TUnique, TKey>
    {
        /// <summary>
        /// CacheKey&lt;TKey&gt; クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="uniqueKey">キャッシュ関連オブジェクトのキー</param>
        /// <param name="accessKey">オブジェクトを区別するキー</param>
        public CacheKey(TUnique uniqueKey, TKey accessKey)
        {
            this.UniqueKey = uniqueKey;
            this.AccessKey = accessKey;
        }

        /// <summary>
        /// キャッシュ関連オブジェクトのキーを取得または設定します。
        /// </summary>
        public TUnique UniqueKey { get; private set; }

        /// <summary>
        /// オブジェクトを区別するキーを取得または設定します。
        /// </summary>
        public TKey AccessKey { get; private set; }
    }
}

