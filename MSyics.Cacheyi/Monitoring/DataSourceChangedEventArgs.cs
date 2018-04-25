/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Collections.ObjectModel;

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
        /// データソースに変更があった場合のキャッシュストアへの要求を取得または設定します。
        /// </summary>
        public DataSourceChangedAction ChangedAction { get; set; }
    }
}
