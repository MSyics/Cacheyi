/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
namespace MSyics.Cacheyi
{
    /// <summary>
    /// 要素の保持状態を表します。
    /// </summary>
    public enum CacheStatus
    {
        /// <summary>
        /// 要素を保持しています。
        /// </summary>
        Real,

        /// <summary>
        /// 要素を保持していません。
        /// </summary>
        Virtual,
    }
}
