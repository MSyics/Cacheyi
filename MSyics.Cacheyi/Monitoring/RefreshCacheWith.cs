/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
namespace MSyics.Cacheyi.Monitoring
{
    /// <summary>
    /// データソースに変更があったときの CacheStore への要求を表します。
    /// </summary>
    public enum RefreshCacheWith
    {
        /// <summary>
        /// 何もしません。
        /// </summary>
        None,
        
        /// <summary>
        /// すべてリセットします。
        /// </summary>
        Reset,

        /// <summary>
        /// 変更箇所をリセットします。
        /// </summary>
        ResetContains,

        /// <summary>
        /// すべて削除します。
        /// </summary>
        Clear,

        /// <summary>
        /// 変更箇所を削除します。
        /// </summary>
        Remove,
     }
}
