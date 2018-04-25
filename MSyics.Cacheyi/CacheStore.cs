/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using MSyics.Cacheyi.Configuration;
using MSyics.Cacheyi.Monitoring;
using System.Threading;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// キャッシュオブジェクトを操作する機能を提供します。
    /// </summary>
    /// <typeparam name="TKeyed">キャッシュするオブジェクトを区別するためのキーの型</typeparam>
    /// <typeparam name="TValue">キャッシュするオブジェクトの型</typeparam>
    public class CacheStore<TKey, TValue>
    {
        internal CacheStore(string name) => Name = name;

        /// <summary>
        /// 指定したキーでキャッシュにアクセスするオブジェクトを取得します。
        /// </summary>
        /// <param name="key">キャッシュを区別するためのキー</param>
        /// <returns>キャッシュを関連付けるオブジェクト</returns>
        public CacheProxy<TKey, TValue> Alloc(TKey key)
        {
            if (key == null) { throw new ArgumentNullException(nameof(key)); }

            using (CacheLock.Scope(LockStatus.UpgradeableRead))
            {
                if (Proxies.Contains(key)) { return Proxies[key]; }

                using (CacheLock.Scope(LockStatus.Write))
                {
                    if (HasMaxCapacity && Proxies.Count >= MaxCapacity)
                    {
                        // 最大容量を超えるときは、最初の要素を削除します。
                        Proxies.RemoveAt(0);
                    }

                    var item = new CacheProxy<TKey, TValue>()
                    {
                        Timeout = Timeout,
                        Key = key,
                        ValueFactoryCallBack = () =>
                        {
                            return new CacheValue<TValue>()
                            {
                                Value = ValueBuilder.GetValue(key),
                                Cached = DateTimeOffset.Now,
                            };
                        },
                    };

                    if (HasTimeout)
                    {
                        item.TimedOutCallBack = () => Remove(key);
                    }

                    Proxies.Add(item);
                    return item;
                }
            }
        }

        /// <summary>
        /// 指定したキーの一覧からキャッシュにアクセスするオブジェクトの一覧を取得します。
        /// </summary>
        /// <param name="keys">キャッシュを区別するためのキーの一覧</param>
        /// <returns>キャッシュを関連付けるオブジェクトの一覧</returns>
        public IEnumerable<CacheProxy<TKey, TValue>> Alloc(IEnumerable<TKey> keys)
        {
            if (keys == null) { yield break; }
            foreach (var item in keys) { yield return Alloc(item); }
        }

        /// <summary>
        /// 指定したキーでキャッシュにアクセスするオブジェクトを取得します。戻り値は取得に成功したかどうかを示します。
        /// </summary>
        /// <param name="key">オブジェクトを区別するためのキー</param>
        /// <param name="cache">キャッシュを関連付けるオブジェクト。取得に失敗した場合は null を設定します。</param>
        /// <returns>取得に成功したかどうかを示す値</returns>
        public bool TryAlloc(TKey key, out CacheProxy<TKey, TValue> cache)
        {
            cache = key == null ? null : Alloc(key);
            return cache != null;
        }

        /// <summary>
        /// すべてのキャッシュを削除します。
        /// </summary>
        public void Clear()
        {
            using (CacheLock.Scope(LockStatus.Write))
            {
                Proxies.Clear();
            }
        }

        private void Reset(IEnumerable<TKey> keys)
        {
            if (keys == null) { return; }

            using (CacheLock.Scope(LockStatus.Read))
            {
                foreach (var cache in Proxies.Join(keys, x => x.Key, y => y, (x, y) => x))
                {
                    cache.Reset();
                };
            }
        }

        /// <summary>
        /// すべてのキャッシュされたオブジェクトとの関連をリセットします。
        /// </summary>
        public void Reset()
        {
            using (CacheLock.Scope(LockStatus.Read))
            {
                foreach (var cache in Proxies)
                {
                    cache.Reset();
                }
            }
        }

        private void Remove(IEnumerable<TKey> keys)
        {
            if (keys == null) { return; }

            using (CacheLock.Scope(LockStatus.Write))
            {
                foreach (var key in keys)
                {
                    Proxies.Remove(key);
                }
            }
        }

        /// <summary>
        /// 指定したキー項目のキャッシュを削除します。
        /// </summary>
        /// <param name="key">削除するキャッシュのキー</param>
        /// <returns>キャッシュが正常に削除された場合は true。それ以外は false。キャッシュが見つからない場合にも false を返します。</returns>
        public bool Remove(TKey key)
        {
            if (key == null) { return true; }

            using (CacheLock.Scope(LockStatus.Write))
            {
                return Proxies.Remove(key);
            }
        }

        internal void OnDataSourceChanged(object sender, DataSourceChangedEventArgs<TKey> e)
        {
            switch (e.ChangedAction)
            {
                case DataSourceChangedAction.Reset:
                    Reset();
                    break;

                case DataSourceChangedAction.ResetContains:
                    if (e.Keys?.Length > 0) { Reset(e.Keys); }
                    break;

                case DataSourceChangedAction.Clear:
                    Clear();
                    break;

                case DataSourceChangedAction.Remove:
                    if (e.Keys?.Length > 0) { Remove(e.Keys); }
                    break;

                case DataSourceChangedAction.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// <para>保持しているキャッシュを圧縮して整理します。</para>
        /// <para>この操作は、実オブジェクトを保持していないキャッシュアクセスオブジェクトを削除します。</para>
        /// </summary>
        public void DoOut()
        {
            using (CacheLock.Scope(LockStatus.Write))
            {
                var items = Proxies.Where(x => x.Status == CacheStatus.Real && x.TimedOut == false).ToArray();
                Proxies.Clear();
                foreach (var item in items)
                {
                    Proxies.Add(item);
                }
            }
        }

        /// <summary>
        /// キャッシュしているオブジェクト数を取得します。
        /// </summary>
        public int Count => Proxies.Count;

        /// <summary>
        /// データソースに変更があったことを通知するオブジェクトを取得します。
        /// </summary>
        public IDataSourceMonitoring<TKey> Monitoring { get; internal set; }

        /// <summary>
        /// データソースの変更通知を受け取ることができるかどうかを示す値を取得します。
        /// </summary>
        public bool CanMonitoring => Monitoring != null;

        /// <summary>
        /// 最大容量が設定されているかどうかを示す値を取得します。
        /// </summary>
        public bool HasMaxCapacity => MaxCapacity > 0;

        /// <summary>
        /// キャッシュオブジェクトの保持期間が設定されているかどうかを示す値を取得します。
        /// </summary>
        public bool HasTimeout => Timeout != TimeSpan.Zero;

        /// <summary>
        /// キャッシュオブジェクトの保持期間を取得します。
        /// </summary>
        public TimeSpan Timeout { get; internal set; } = TimeSpan.Zero;

        /// <summary>
        /// インスタンスの最大キャッシュ容量を取得します。
        /// </summary>
        public int MaxCapacity { get; internal set; } = 0;

        internal string Name { get; private set; }
        internal ICacheValueBuilder<TKey, TValue> ValueBuilder { get; set; }

        internal ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();
        internal CacheProxyCollection<TKey, TValue> Proxies = new CacheProxyCollection<TKey, TValue>();

        /// <summary>
        /// Finalyzer
        /// </summary>
        ~CacheStore()
        {
            if (CanMonitoring && Monitoring.Running) { Monitoring.Stop(); }
            CacheLock.Dispose();
        }
    }

    public class CacheStore<TKeyed, TKey, TValue> : CacheStore<TKey, TValue>
    {
        internal CacheStore(string name) : base(name)
        {
        }

        public CacheProxy<TKey, TValue> Alloc(TKeyed keyed) => Alloc(KeyBuilder.GetKey(keyed));

        public IEnumerable<CacheProxy<TKey, TValue>> Alloc(IEnumerable<TKeyed> keyeds) => Alloc(keyeds.Select(x => KeyBuilder.GetKey(x)));

        public bool Remove(TKeyed keyed) => Remove(KeyBuilder.GetKey(keyed));

        public bool TryAlloc(TKeyed keyed, out CacheProxy<TKey, TValue> cache) => TryAlloc(KeyBuilder.GetKey(keyed), out cache);

        internal ICacheKeyBuilder<TKeyed, TKey> KeyBuilder { get; set; }
    }
}