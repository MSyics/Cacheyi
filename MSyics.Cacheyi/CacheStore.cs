/****************************************************************
© 2018 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using MSyics.Cacheyi.Configuration;
using MSyics.Cacheyi.Monitors;
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
                if (Caches.Contains(key)) { return Caches[key]; }

                using (CacheLock.Scope(LockStatus.Write))
                {
                    if (HasMaxCapacity && Caches.Count >= MaxCapacity)
                    {
                        // 最大容量を超えるときは、最初の要素を削除します。
                        Caches.RemoveAt(0);
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

                    Caches.Add(item);
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
                Caches.Clear();
            }
        }

        private void Reset(IEnumerable<TKey> keys)
        {
            if (keys == null) { return; }

            using (CacheLock.Scope(LockStatus.Read))
            {
                foreach (var cache in Caches.Join(keys, x => x.Key, y => y, (x, y) => x))
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
                foreach (var cache in Caches)
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
                    Caches.Remove(key);
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
                return Caches.Remove(key);
            }
        }

        /// <summary>
        /// オブジェクト変更通知機能を登録している場合に変更通知を開始します。
        /// </summary>
        public void StartMonitoring()
        {
            if (CanMonitoring)
            {
                ChangeMonitor.Changed += new EventHandler<DataSourceChangeEventArgs<TKey>>(DataSourceChangeMonitor_Changed);
                ChangeMonitor.Start();
            }
        }

        /// <summary>
        /// オブジェクト変更通知機能を登録している場合に変更通知を停止します。
        /// </summary>
        public void StopMonitoring()
        {
            if (CanMonitoring)
            {
                ChangeMonitor.Changed -= new EventHandler<DataSourceChangeEventArgs<TKey>>(DataSourceChangeMonitor_Changed);
                ChangeMonitor.Stop();
            }
        }

        private void DataSourceChangeMonitor_Changed(object sender, DataSourceChangeEventArgs<TKey> e)
        {
            switch (e.ChangeAction)
            {
                case CacheChangeAction.Reset:
                    Reset();
                    break;

                case CacheChangeAction.ResetContains:
                    if (e.Keys != null || e.Keys.Count > 0) { Reset(e.Keys); }
                    break;

                case CacheChangeAction.Clear:
                    Clear();
                    break;

                case CacheChangeAction.Remove:
                    if (e.Keys != null || e.Keys.Count > 0) { Remove(e.Keys); }
                    break;

                case CacheChangeAction.None:
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
                var items = Caches.Where(x => x.Status == CacheStatus.Real && x.TimedOut == false).ToArray();
                Caches.Clear();
                foreach (var item in items)
                {
                    Caches.Add(item);
                }
            }
        }

        /// <summary>
        /// データソースに変更があったことを通知するオブジェクトを取得します。
        /// </summary>
        public IDataSourceChangeMonitor<TKey> ChangeMonitor { get; internal set; }

        /// <summary>
        /// キャッシュしているオブジェクト数を取得します。
        /// </summary>
        public int Count => Caches.Count;

        /// <summary>
        /// データソースの変更通知を受け取ることができるかどうかを示す値を取得します。
        /// </summary>
        public bool CanMonitoring => ChangeMonitor != null;

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
        internal CacheKeyedCollection<TKey, TValue> Caches = new CacheKeyedCollection<TKey, TValue>();

        /// <summary>
        /// Finalyzer
        /// </summary>
        ~CacheStore()
        {
            if (CanMonitoring)
            {
                StopMonitoring();
            }

            CacheLock.Dispose();
        }
    }

    public class CacheStore<TKeyed, TKey, TValue> : CacheStore<TKey, TValue>
    {
        internal CacheStore(string name) : base(name)
        {
        }

        public CacheProxy<TKey, TValue> Alloc(TKeyed keyed)
        {
            return Alloc(KeyBuilder.GetKey(keyed));
        }

        public IEnumerable<CacheProxy<TKey, TValue>> Alloc(IEnumerable<TKeyed> keyeds)
        {
            return Alloc(keyeds.Select(x => KeyBuilder.GetKey(x)));
        }

        public bool Remove(TKeyed keyed)
        {
            return Remove(KeyBuilder.GetKey(keyed));
        }

        public bool TryAlloc(TKeyed keyed, out CacheProxy<TKey, TValue> cache)
        {
            return TryAlloc(KeyBuilder.GetKey(keyed), out cache);
        }

        internal ICacheKeyBuilder<TKeyed, TKey> KeyBuilder { get; set; }
    }
}