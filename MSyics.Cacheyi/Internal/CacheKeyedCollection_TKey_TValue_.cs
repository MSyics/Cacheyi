/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Collections.ObjectModel;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュ関連オブジェクトを格納するクラスです。
    /// </summary>
    /// <typeparam name="TKey">キー</typeparam>
    /// <typeparam name="TValue">キャッシュ</typeparam>
    internal sealed class CacheKeyedCollection<TKey, TValue> : KeyedCollection<int, CacheProxy<TKey, TValue>>
    {
        public CacheKeyedCollection() : base(null, 0) { }

        protected override int GetKeyForItem(CacheProxy<TKey, TValue> item)
        {
            return item.CacheKey.UniqueKey;
        }

        protected override void RemoveItem(int index)
        {
            base.RemoveItem(index);
        }
    }
}
