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
    interface ICacheStore
    {
        bool CanMonitoring { get; }
        bool HasMaxCapacity { get; }
        bool HasTimeout { get; }
        int Count { get; }
        int MaxCapacity { get; }
        TimeSpan Timeout { get; }

        void Clear();
        void DoOut();
        void Reset();
        void StartMonitoring();
        void StopMonitoring();
    }

    interface ICacheStore<TKey>
    {
        IDataSourceChangeMonitor<TKey> ChangeMonitor { get; }
    }

    interface ICacheStore<TKey, TValue> : ICacheStore, ICacheStore<TKey>
    {
        CacheProxy<TKey, TValue> Alloc(TKey key);
        bool TryAlloc(TKey key, out CacheProxy<TKey, TValue> cache);
        IEnumerable<CacheProxy<TKey, TValue>> Alloc(IEnumerable<TKey> keys);
        bool Remove(TKey key);
    }

    interface ICacheStore<TKeyed, TKey, TValue> : ICacheStore, ICacheStore<TKey>
    {
        CacheProxy<TKey, TValue> Alloc(TKeyed keyed);
        bool TryAlloc(TKeyed keyed, out CacheProxy<TKey, TValue> cache);
        IEnumerable<CacheProxy<TKey, TValue>> Alloc(IEnumerable<TKeyed> keyeds);
        bool Remove(TKeyed keyed);
    }

    /// <summary>
    /// キャッシュオブジェクトを操作する機能を提供します。
    /// </summary>
    /// <typeparam name="TKeyed">キャッシュするオブジェクトを区別するためのキーの型</typeparam>
    /// <typeparam name="TValue">キャッシュするオブジェクトの型</typeparam>
    public sealed class CacheStore<TKeyed, TKey, TValue> : ICacheStore<TKeyed, TKey, TValue>
    {
        internal CacheStore(string name) => Name = name;

        /// <summary>
        /// 指定したキーでキャッシュにアクセスするオブジェクトを取得します。
        /// </summary>
        /// <param name="keyed">キャッシュを区別するためのキー</param>
        /// <returns>キャッシュを関連付けるオブジェクト</returns>
        public CacheProxy<TKey, TValue> Alloc(TKeyed keyed)
        {
            if (keyed == null) { throw new ArgumentNullException(nameof(keyed)); }

            using (CacheLock.Scope(LockStatus.UpgradeableRead))
            {
                var key = KeyBuilder.GetKey(keyed);

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
                                Value = ValueBuilder.GetValue(keyed),
                                Cached = DateTimeOffset.Now,
                            };
                        },
                    };

                    if (HasTimeout)
                    {
                        item.TimedOutCallBack = () => Remove(keyed);
                    }

                    Caches.Add(item);
                    return item;
                }
            }
        }

        /// <summary>
        /// 指定したキーの一覧からキャッシュにアクセスするオブジェクトの一覧を取得します。
        /// </summary>
        /// <param name="keyeds">キャッシュを区別するためのキーの一覧</param>
        /// <returns>キャッシュを関連付けるオブジェクトの一覧</returns>
        public IEnumerable<CacheProxy<TKey, TValue>> Alloc(IEnumerable<TKeyed> keyeds)
        {
            if (keyeds == null) { yield break; }
            foreach (var item in keyeds) { yield return Alloc(item); }
        }

        /// <summary>
        /// 指定したキーでキャッシュにアクセスするオブジェクトを取得します。戻り値は取得に成功したかどうかを示します。
        /// </summary>
        /// <param name="keyed">オブジェクトを区別するためのキー</param>
        /// <param name="cache">キャッシュを関連付けるオブジェクト。取得に失敗した場合は null を設定します。</param>
        /// <returns>取得に成功したかどうかを示す値</returns>
        public bool TryAlloc(TKeyed keyed, out CacheProxy<TKey, TValue> cache)
        {
            cache = keyed == null ? null : Alloc(keyed);
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
        /// <param name="keyed">削除するキャッシュのキー</param>
        /// <returns>キャッシュが正常に削除された場合は true。それ以外は false。キャッシュが見つからない場合にも false を返します。</returns>
        public bool Remove(TKeyed keyed)
        {
            if (keyed == null) { return true; }

            using (CacheLock.Scope(LockStatus.Write))
            {
                return Caches.Remove(KeyBuilder.GetKey(keyed));
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
        internal ICacheKeyBuilder<TKeyed, TKey> KeyBuilder { get; set; }
        internal ICacheValueBuilder<TKeyed, TValue> ValueBuilder { get; set; }

        private ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();
        private CacheKeyedCollection<TKey, TValue> Caches = new CacheKeyedCollection<TKey, TValue>();

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

    /// <summary>
    /// キャッシュオブジェクトを操作する機能を提供します。
    /// </summary>
    /// <typeparam name="TKey">キャッシュするオブジェクトを区別するためのキーの型</typeparam>
    /// <typeparam name="TValue">キャッシュするオブジェクトの型</typeparam>
    public sealed class CacheStore<TKey, TValue> : ICacheStore<TKey, TValue>
    {
        private CacheStore<TKey, TKey, TValue> InternalStore { get; }

        internal CacheStore(string name) => InternalStore = new CacheStore<TKey, TKey, TValue>(name);

        internal string Name => InternalStore.Name;
        internal ICacheKeyBuilder<TKey, TKey> KeyBuilder { get => InternalStore.KeyBuilder; set => InternalStore.KeyBuilder = value; }
        internal ICacheValueBuilder<TKey, TValue> ValueBuilder { get => InternalStore.ValueBuilder; set => InternalStore.ValueBuilder = value; }

        public IDataSourceChangeMonitor<TKey> ChangeMonitor { get => InternalStore.ChangeMonitor; internal set => InternalStore.ChangeMonitor = value; }

        public bool CanMonitoring => InternalStore.CanMonitoring;

        public int Count => InternalStore.Count;

        public bool HasMaxCapacity => InternalStore.HasMaxCapacity;

        public bool HasTimeout => InternalStore.HasTimeout;

        public int MaxCapacity { get => InternalStore.MaxCapacity; set => InternalStore.MaxCapacity = value; }

        public TimeSpan Timeout { get => InternalStore.Timeout; internal set => InternalStore.Timeout = value; }

        public IEnumerable<CacheProxy<TKey, TValue>> Alloc(IEnumerable<TKey> keys) => InternalStore.Alloc(keys);

        public CacheProxy<TKey, TValue> Alloc(TKey key) => InternalStore.Alloc(key);

        public void Clear() => InternalStore.Clear();

        public void DoOut() => InternalStore.DoOut();

        public bool Remove(TKey key) => InternalStore.Remove(key);

        public void Reset() => InternalStore.Reset();

        public void StartMonitoring() => InternalStore.StartMonitoring();

        public void StopMonitoring() => InternalStore.StopMonitoring();

        public bool TryAlloc(TKey key, out CacheProxy<TKey, TValue> cache) => InternalStore.TryAlloc(key, out cache);
    }
}