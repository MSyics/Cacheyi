/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Monitors
{
    /// <summary>
    /// データソースに変更があったことを通知する機能を実装します。
    /// </summary>
    /// <typeparam name="TKey">キー</typeparam>
    public interface IDataSourceChangeMonitor<TKey>
    {
        /// <summary>
        /// データソースに変更があったときに発生します。
        /// </summary>
        event EventHandler<DataSourceChangeEventArgs<TKey>> Changed;

        /// <summary>
        /// データソースの監視を開始します。
        /// </summary>
        void Start();

        /// <summary>
        /// データソースの監視を停止します。
        /// </summary>
        void Stop();

        /// <summary>
        /// 監視待機中かどうかを示す値を取得します。
        /// </summary>
        bool Waiting { get; }
    }
}
