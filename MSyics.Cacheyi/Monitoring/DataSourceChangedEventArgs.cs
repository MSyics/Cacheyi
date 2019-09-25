using System;

namespace MSyics.Cacheyi.Monitoring
{
    /// <summary>
    /// データソースに変更があったときに発生するイベントデータを格納します。
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class DataSourceChangedEventArgs<TKey> : EventArgs
    {
        /// <summary>
        /// データソースで変更があった要素のキーを取得または設定します。
        /// </summary>
        public TKey[] Keys { get; set; }

        /// <summary>
        /// データソースに変更があったときの CacheStore への要求を取得または設定します。
        /// </summary>
        public RefreshCacheWith RefreshWith { get; set; }
    }
}
