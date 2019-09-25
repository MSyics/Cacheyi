using System;

namespace MSyics.Cacheyi.Monitoring
{
    /// <summary>
    /// データソースに変更があったことを通知する機能を実装します。
    /// </summary>
    /// <typeparam name="TKey">キー</typeparam>
    public interface IDataSourceMonitoring<TKey>
    {
        /// <summary>
        /// データソースに変更があったときに発生します。
        /// </summary>
        event EventHandler<DataSourceChangedEventArgs<TKey>> Changed;

        /// <summary>
        /// データソースの監視を開始します。
        /// </summary>
        void Start();

        /// <summary>
        /// データソースの監視を停止します。
        /// </summary>
        void Stop();

        /// <summary>
        /// 監視中かどうかを示す値を取得します。監視中の場合は true、それ以外は false。
        /// </summary>
        bool Running { get; }
    }
}
