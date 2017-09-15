/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュされるオブジェクトとの関連を表します。
    /// </summary>
    public enum CacheStatus
    {
        /// <summary>
        /// オブジェクトを保持しています。
        /// </summary>
        Real,

        /// <summary>
        /// オブジェクトをまだ保持していません。
        /// </summary>
        Virtual,
    }
}
