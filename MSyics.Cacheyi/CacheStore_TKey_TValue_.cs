﻿/****************************************************************
© 2016 MSyics
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
    /// <typeparam name="TKey">キャッシュするオブジェクトを区別するためのキーの型</typeparam>
    /// <typeparam name="TValue">キャッシュするオブジェクトの型</typeparam>
    public class CacheStore<TKey, TValue>
    {
        internal CacheStore(string name)
        {
            this.Name = name;
        }

        private void DataSourceChangeMonitor_OnChanged(object sender, DataSourceChangeEventArgs<TKey> e)
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

        private CacheProxy<TKey, TValue> Alloc(CacheKey<TKey> key)
        {
            if (m_caches.Contains(key.UniqueKey)) { return m_caches[key.UniqueKey]; }

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
                    CacheKey = key,
                    ValueFactoryCallBack = () =>
                    {
                        return new CacheValue<TValue>()
                        {
                            Value = this.ValueBuilder.GetValue(key.AccessKey),
                            Created = DateTimeOffset.Now,
                        };
                    },
                };

                if (this.HasTimeout)
                {
                    item.TimedOutCallBack = () => this.Remove(key.AccessKey);
                }

                m_caches.Add(item);
                return item;
            }
        }

        /// <summary>
        /// 指定したキーでキャッシュにアクセスするオブジェクトを取得します。
        /// </summary>
        /// <param name="key">キャッシュを区別するためのキー</param>
        /// <returns>キャッシュを関連付けるオブジェクト</returns>
        public CacheProxy<TKey, TValue> Alloc(TKey key)
        {
            #region Doer
            if (key == null) { throw new ArgumentNullException(nameof(key)); }
            #endregion

            using (m_cacheLock.Scope(LockStatus.UpgradeableRead))
            {
                return Alloc(this.KeyBuilder.GetCacheKey(key));
            }
        }

        /// <summary>
        /// 指定したキーの一覧からキャッシュにアクセスするオブジェクトの一覧を取得します。
        /// </summary>
        /// <param name="keys">キャッシュを区別するためのキーの一覧</param>
        /// <returns>キャッシュを関連付けるオブジェクトの一覧</returns>
        public IEnumerable<CacheProxy<TKey, TValue>> Alloc(IEnumerable<TKey> keys)
        {
            if (keys == null)
            {
                yield break;
            }

            foreach (var item in keys)
            {
                yield return Alloc(item);
            }
        }

        /// <summary>
        /// 指定したキーでキャッシュにアクセスするオブジェクトを取得します。戻り値は取得に成功したかどうかを示します。
        /// </summary>
        /// <param name="key">オブジェクトを区別するためのキー</param>
        /// <param name="cache">キャッシュを関連付けるオブジェクト。取得に失敗した場合は null を設定します。</param>
        /// <returns>取得に成功したかどうかを示す値</returns>
        public bool TryAlloc(TKey key, out CacheProxy<TKey, TValue> cache)
        {
            if (key == null)
            {
                cache = null;
            }
            else
            {
                cache = Alloc(key);
            }
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

        private void Reset(IEnumerable<TKey> keys)
        {
            if (keys == null) { return; }

            using (m_cacheLock.Scope(LockStatus.Read))
            {
                foreach (var key in keys)
                {
                    var uniqueKey = this.KeyBuilder.GetCacheKey(key).UniqueKey;
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

        private void Remove(IEnumerable<TKey> keys)
        {
            if (keys == null) { return; }

            using (m_cacheLock.Scope(LockStatus.Write))
            {
                foreach (var key in keys)
                {
                    m_caches.Remove(this.KeyBuilder.GetCacheKey(key).UniqueKey);
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

            using (m_cacheLock.Scope(LockStatus.Write))
            {
                return m_caches.Remove(this.KeyBuilder.GetCacheKey(key).UniqueKey);
            }
        }

        /// <summary>
        /// オブジェクト変更通知機能を登録している場合に変更通知を開始します。
        /// </summary>
        public void StartMonitoring()
        {
            if (this.CanMonitoring)
            {
                this.ChangeMonitor.OnChanged += new EventHandler<DataSourceChangeEventArgs<TKey>>(DataSourceChangeMonitor_OnChanged);
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
                this.ChangeMonitor.OnChanged -= new EventHandler<DataSourceChangeEventArgs<TKey>>(DataSourceChangeMonitor_OnChanged);
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
        internal ICacheKeyBuilder<TKey> KeyBuilder { get; set; }
        internal ICacheValueBuilder<TKey, TValue> ValueBuilder { get; set; }

        /// <summary>
        /// データソースに変更があったことを通知するオブジェクトを取得します。
        /// </summary>
        public IDataSourceChangeMonitor<TKey> ChangeMonitor { get; internal set; }

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
}