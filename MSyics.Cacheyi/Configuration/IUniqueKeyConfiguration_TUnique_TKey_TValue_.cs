﻿/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// キーについての設定を行います。
    /// </summary>
    public interface IUniqueKeyConfiguration<TUnique, TKey, TValue>
    {
        /// <summary>
        /// キャッシュ関連オブジェクトを区別するキーを派生するオブジェクトを登録します。
        /// </summary>
        /// <param name="builder">キーを派生する機能を提供するデリゲート</param>
        IValueConfiguration<TKey, TValue> MakeUniqueKey(Func<TKey, TUnique> builder);
    }
}