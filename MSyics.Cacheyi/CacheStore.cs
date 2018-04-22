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
    public class CacheStore<TKeyed, TKey, TValue>
    {
        internal CacheStore(string name)
        {
            this.Name = name;
        }

        private void DataSourceChangeMonitor_Changed(object sender, DataSourceChangeEventArgs<TKeyed> e)
        {
            switch (e.ChangeAction)
            {
                case CacheChangeAction.Reset:
                    this.Reset();
                    break;

                case CacheChangeAction.ResetContains:
                    if (e.Keys != null || e.Keys.Count > 0)
                    {
                        this.Reset(e.Keys);
                    }
                    break;

                case CacheChangeAction.Clear:
                    this.Clear();
                    break;

                case CacheChangeAction.Remove:
                    if (e.Keys != null || e.Keys.Count > 0)
                    {
                        this.Remove(e.Keys);
                    }
                    break;

                case CacheChangeAction.None:
                default:
                    break;
            }
        }

        private CacheProxy<TKey, TValue> InternalAlloc(TKeyed keyed)
        {
            var key = this.KeyBuilder.GetKey(keyed);

            if (m_caches.Contains(key)) { return m_caches[key]; }

            using (m_cacheLock.Scope(LockStatus.Write))
            {
                if (this.HasMaxCapacity && m_caches.Count >= this.MaxCapacity)
                {
                    // 最大容量を超えるときは、最初の要素を削除します。
                    m_caches.RemoveAt(0);
                }

                var item = new CacheProxy<TKey, TValue>()
                {
                    Timeout = this.Timeout,
                    Key = key,
                    ValueFactoryCallBack = () =>
                    {
                        return new CacheValue<TValue>()
                        {
                            Value = this.ValueBuilder.GetValue(keyed),
                            Cached = DateTimeOffset.Now,
                        };
                    },
                };

                if (this.HasTimeout)
                {
                    item.TimedOutCallBack = () => this.Remove(keyed);
                }

                m_caches.Add(item);
                return item;
            }
        }

        /// <summary>
        /// 指定したキーでキャッシュにアクセスするオブジェクトを取得します。
        /// </summary>
        /// <param name="keyed">キャッシュを区別するためのキー</param>
        /// <returns>キャッシュを関連付けるオブジェクト</returns>
        public CacheProxy<TKey, TValue> Alloc(TKeyed keyed)
        {
            #region Doer
            if (keyed == null) { throw new ArgumentNullException(nameof(keyed)); }
            #endregion

            using (m_cacheLock.Scope(LockStatus.UpgradeableRead))
            {
                return InternalAlloc(keyed);
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
            foreach (var item in keyeds) { yield return InternalAlloc(item); }
        }

        /// <summary>
        /// 指定したキーでキャッシュにアクセスするオブジェクトを取得します。戻り値は取得に成功したかどうかを示します。
        /// </summary>
        /// <param name="keyed">オブジェクトを区別するためのキー</param>
        /// <param name="cache">キャッシュを関連付けるオブジェクト。取得に失敗した場合は null を設定します。</param>
        /// <returns>取得に成功したかどうかを示す値</returns>
        public bool TryAlloc(TKeyed keyed, out CacheProxy<TKey, TValue> cache)
        {
            cache = keyed == null ? null : InternalAlloc(keyed);
            return cache != null;
        }

        /// <summary>
        /// すべてのキャッシュを削除します。
        /// </summary>
        public void Clear()
        {
            using (m_cacheLock.Scope(LockStatus.Write))
            {
                m_caches.Clear();
            }
        }

        private void Reset(IEnumerable<TKeyed> keyeds)
        {
            if (keyeds == null) { return; }

            using (m_cacheLock.Scope(LockStatus.Read))
            {
                foreach (var key in keyeds)
                {
                    var uniqueKey = this.KeyBuilder.GetKey(key);
                    if (m_caches.Contains(uniqueKey))
                    {
                        m_caches[uniqueKey].Reset();
                    }
                };
            }
        }

        /// <summary>
        /// すべてのキャッシュされたオブジェクトとの関連をリセットします。
        /// </summary>
        public void Reset()
        {
            using (m_cacheLock.Scope(LockStatus.Read))
            {
                foreach (var cache in m_caches)
                {
                    cache.Reset();
                }
            }
        }

        private void Remove(IEnumerable<TKeyed> keyeds)
        {
            if (keyeds == null) { return; }

            using (m_cacheLock.Scope(LockStatus.Write))
            {
                foreach (var key in keyeds)
                {
                    m_caches.Remove(this.KeyBuilder.GetKey(key));
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

            using (m_cacheLock.Scope(LockStatus.Write))
            {
                return m_caches.Remove(this.KeyBuilder.GetKey(keyed));
            }
        }

        /// <summary>
        /// オブジェクト変更通知機能を登録している場合に変更通知を開始します。
        /// </summary>
        public void StartMonitoring()
        {
            if (this.CanMonitoring)
            {
                this.ChangeMonitor.Changed += new EventHandler<DataSourceChangeEventArgs<TKeyed>>(DataSourceChangeMonitor_Changed);
                this.ChangeMonitor.Start();
            }
        }

        /// <summary>
        /// オブジェクト変更通知機能を登録している場合に変更通知を停止します。
        /// </summary>
        public void StopMonitoring()
        {
            if (this.CanMonitoring)
            {
                this.ChangeMonitor.Changed -= new EventHandler<DataSourceChangeEventArgs<TKeyed>>(DataSourceChangeMonitor_Changed);
                this.ChangeMonitor.Stop();
            }
        }

        /// <summary>
        /// <para>保持しているキャッシュを圧縮して整理します。</para>
        /// <para>この操作は、実オブジェクトを保持していないキャッシュアクセスオブジェクトを削除します。</para>
        /// </summary>
        public void DoOut()
        {
            using (m_cacheLock.Scope(LockStatus.Write))
            {
                var items = m_caches.Where(x => x.Status == CacheStatus.Real && x.TimedOut == false).ToArray();
                m_caches.Clear();
                foreach (var item in items)
                {
                    m_caches.Add(item);
                }
            }
        }

        internal string Name { get; private set; }
        internal ICacheKeyBuilder<TKeyed, TKey> KeyBuilder { get; set; }
        internal ICacheValueBuilder<TKeyed, TValue> ValueBuilder { get; set; }

        /// <summary>
        /// データソースに変更があったことを通知するオブジェクトを取得します。
        /// </summary>
        public IDataSourceChangeMonitor<TKeyed> ChangeMonitor { get; internal set; }

        /// <summary>
        /// キャッシュしているオブジェクト数を取得します。
        /// </summary>
        public int Count => this.m_caches.Count;

        /// <summary>
        /// データソースの変更通知を受け取ることができるかどうかを示す値を取得します。
        /// </summary>
        public bool CanMonitoring => this.ChangeMonitor != null;

        /// <summary>
        /// 最大容量が設定されているかどうかを示す値を取得します。
        /// </summary>
        public bool HasMaxCapacity => this.MaxCapacity > 0;

        /// <summary>
        /// キャッシュオブジェクトの保持期間が設定されているかどうかを示す値を取得します。
        /// </summary>
        public bool HasTimeout => this.Timeout != TimeSpan.Zero;

        /// <summary>
        /// キャッシュオブジェクトの保持期間を取得します。
        /// </summary>
        public TimeSpan Timeout { get; internal set; } = TimeSpan.Zero;

        /// <summary>
        /// インスタンスの最大キャッシュ容量を取得します。
        /// </summary>
        public int MaxCapacity { get; internal set; } = 0;

        private ReaderWriterLockSlim m_cacheLock = new ReaderWriterLockSlim();
        private CacheKeyedCollection<TKey, TValue> m_caches = new CacheKeyedCollection<TKey, TValue>();

        /// <summary>
        /// Finalyzer
        /// </summary>
        ~CacheStore()
        {
            if (CanMonitoring)
            {
                StopMonitoring();
            }

            this.m_cacheLock.Dispose();
        }
    }

    /// <summary>
    /// キャッシュオブジェクトを操作する機能を提供します。
    /// </summary>
    /// <typeparam name="TKey">キャッシュするオブジェクトを区別するためのキーの型</typeparam>
    /// <typeparam name="TValue">キャッシュするオブジェクトの型</typeparam>
    public class CacheStore<TKey, TValue> : CacheStore<TKey, TKey, TValue>
    {
        internal CacheStore(string name) : base(name) { }
    }
}