/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// 取得する値についての設定をします。
    /// </summary>
    public interface IValueConfiguration<TKey, TValue>
    {
        /// <summary>
        /// データソースからオブジェクトを取得する機能を登録します。
        /// </summary>
        /// <param name="builder">データソースからオブジェクトを取得する機能を提供するデリゲート</param>
        void MakeValue(Func<TKey, TValue> builder);
    }
}
