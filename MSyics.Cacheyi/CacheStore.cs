using MSyics.Cacheyi.Monitoring;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// 要素を保持します。
    /// </summary>
    /// <typeparam name="TKeyed">要素のキーを保有する型</typeparam>
    /// <typeparam name="TKey">要素を選別するキーの型</typeparam>
    /// <typeparam name="TValue">要素の型</typeparam>
    public interface ICacheStore<TKeyed, TKey, TValue>
    {
        /// <summary>
        /// 要素の最大保持量を取得します。
        /// </summary>
        int MaxCapacity { get; }

        /// <summary>
        /// /最大保持量を持っているかどうかを示す値を取得します。
        /// </summary>
        bool HasMaxCapacity { get; }

        /// <summary>
        /// 要素の保持期間を取得します。
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// 保持期間を持っているかどうかを示す値を取得します。
        /// </summary>
        bool HasTimeout { get; }

        /// <summary>
        /// データソース監視オブジェクトを取得します。
        /// </summary>
        IDataSourceMonitoring<TKey> Monitoring { get; }

        /// <summary>
        /// データソース監視ができるかどうかを示す値を取得します。
        /// </summary>
        bool CanMonitoring { get; }

        /// <summary>
        /// 要素の一覧を取得します。
        /// </summary>
        IEnumerable<CacheProxy<TKey, TValue>> AsEnumerable();

        /// <summary>
        /// 要素の保持数を取得します。
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 保持している要素すべて削除します。
        /// </summary>
        void Clear();

        /// <summary>
        /// <para>保持している要素を圧縮して整理します。</para>
        /// <para>この操作は、実要素を保持していない要素を削除します。</para>
        /// </summary>
        void DoOut();

        /// <summary>
        /// すべての要素をリセットします。
        /// </summary>
        void Reset();

        /// <summary>
        /// 指定したオブジェクトから要素を引き当てます。
        /// </summary>
        /// <param name="keyed">キーを保有するオブジェクト</param>
        CacheProxy<TKey, TValue> Alloc(TKeyed keyed);

        /// <summary>
        /// 指定したオブジェクトの一覧から要素を引き当てます。
        /// </summary>
        /// <param name="keyeds">キーを保有するオブジェクトの一覧</param>
        CacheProxy<TKey, TValue>[] Alloc(IEnumerable<TKeyed> keyeds);

        /// <summary>
        /// 指定した要素を登録します。
        /// </summary>
        /// <param name="keyed">キーを保有するオブジェクト</param>
        /// <param name="value">登録する要素</param>
        CacheProxy<TKey, TValue> Alloc(TKeyed keyed, TValue value);

        /// <summary>
        /// 指定したオブジェクトで要素の引当を試みます。
        /// </summary>
        /// <param name="keyed">キーを保有するオブジェクト</param>
        /// <param name="cache">要素</param>
        bool TryAlloc(TKeyed keyed, out CacheProxy<TKey, TValue> cache);

        /// <summary>
        /// 指定したオブジェクトで要素を削除します。
        /// </summary>
        /// <param name="keyed">キーを保有するオブジェクト</param>
        bool Remove(TKeyed keyed);

        /// <summary>
        /// 指定したオブジェクトの一覧から要素を削除します。
        /// </summary>
        /// <param name="keyeds">キーを保有するオブジェクトの一覧</param>
        void Remove(IEnumerable<TKeyed> keyeds);
    }

    /// <summary>
    /// 要素を保持します。
    /// </summary>
    /// <typeparam name="TKey">要素を選別するキーの型</typeparam>
    /// <typeparam name="TValue">要素の型</typeparam>
    public interface ICacheStore<TKey, TValue> : ICacheStore<TKey, TKey, TValue>
    {
        /// <summary>
        /// 指定したキーから要素を引き当てます。
        /// </summary>
        /// <param name="key">キー</param>
        new CacheProxy<TKey, TValue> Alloc(TKey key);

        /// <summary>
        /// 指定したキーの一覧から要素を引き当てます。
        /// </summary>
        /// <param name="keys">キーの一覧</param>
        new CacheProxy<TKey, TValue>[] Alloc(IEnumerable<TKey> keys);

        /// <summary>
        /// 指定したキーで要素の引当を試みます。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="cache">要素</param>
        new bool TryAlloc(TKey key, out CacheProxy<TKey, TValue> cache);

        /// <summary>
        /// 指定したキーで要素を削除します。
        /// </summary>
        /// <param name="key">キー</param>
        new bool Remove(TKey key);

        /// <summary>
        /// 指定したキーの一覧から要素を削除します。
        /// </summary>
        /// <param name="keys">キーの一覧</param>
        new void Remove(IEnumerable<TKey> keys);

        /// <summary>
        /// 指定した要素を登録します。
        /// </summary>
        /// <param name="key">キー</param>
        /// <param name="value">登録する要素</param>
        new CacheProxy<TKey, TValue> Alloc(TKey key, TValue value);
    }

    internal class InternalCacheStore<TKeyed, TKey, TValue> : ICacheStore<TKeyed, TKey, TValue>
    {
        internal ICacheKeyBuilder<TKeyed, TKey> KeyBuilder { get; set; }
        internal ICacheValueBuilder<TKeyed, TKey, TValue> ValueBuilder { get; set; }

        private readonly ReaderWriterLockSlim LockSlim = new ReaderWriterLockSlim();
        private readonly CacheProxyCollection<TKey, TValue> CacheProxies = new CacheProxyCollection<TKey, TValue>();

        internal InternalCacheStore()
        {
        }

        internal InternalCacheStore(ICacheKeyBuilder<TKeyed, TKey> keyBuilder, ICacheValueBuilder<TKeyed, TKey, TValue> valueBuilder)
        {
            KeyBuilder = keyBuilder;
            ValueBuilder = valueBuilder;
        }

        internal InternalCacheStore(ICacheKeyBuilder<TKeyed, TKey> keyBuilder, ICacheValueBuilder<TKeyed, TKey, TValue> valueBuilder, IDataSourceMonitoring<TKey> monitoring) :
            this(keyBuilder, valueBuilder)
        {
            Monitoring = monitoring;
            if (Monitoring != null)
            {
                Monitoring.Changed += OnDataSourceChanged;
            }
        }

        ~InternalCacheStore()
        {
            if (CanMonitoring && Monitoring.Running) { Monitoring.Stop(); }
            LockSlim.Dispose();
        }

        private CacheProxy<TKey, TValue> AddCachProxy(TKey key, Func<CacheValue<TValue>> getValueCallback)
        {
            if (HasMaxCapacity && CacheProxies.Count >= MaxCapacity)
            {
                // 最大容量を超えるときは、最初の要素を削除します。
                CacheProxies.RemoveAt(0);
            }
            var item = new CacheProxy<TKey, TValue>()
            {
                Timeout = Timeout,
                Key = key,
                GetValueCallBack = getValueCallback,
            };
            if (HasTimeout)
            {
                item.TimedOutCallBack = () => Remove(key);
            }
            CacheProxies.Add(item);
            return item;
        }

        public CacheProxy<TKey, TValue> Alloc(TKeyed keyed, TValue value)
        {
            using (LockSlim.Scope(LockStatus.UpgradeableRead))
            {
                var key = KeyBuilder.GetKey(keyed);
                CacheProxies.Remove(key);
                using (LockSlim.Scope(LockStatus.Write))
                {
                    var proxy = AddCachProxy(key, () => new CacheValue<TValue>()
                    {
                        Value = value,
                        Cached = DateTimeOffset.Now,
                    });
                    return proxy;
                }
            }
        }

        public CacheProxy<TKey, TValue> Alloc(TKeyed keyed)
        {
            if (keyed == null) { new ArgumentNullException(nameof(keyed)); }
            using (LockSlim.Scope(LockStatus.UpgradeableRead))
            {
                var key = KeyBuilder.GetKey(keyed);
                if (CacheProxies.Contains(key)) { return CacheProxies[key]; }
                using (LockSlim.Scope(LockStatus.Write))
                {
                    var proxy = AddCachProxy(key, () => new CacheValue<TValue>()
                    {
                        Value = ValueBuilder.GetValue(keyed, key),
                        Cached = DateTimeOffset.Now,
                    });
                    return proxy;
                }
            }
        }

        public CacheProxy<TKey, TValue>[] Alloc(IEnumerable<TKeyed> keyeds)
        {
            if (keyeds == null)
            {
                return Enumerable.
                    Empty<CacheProxy<TKey, TValue>>().
                    ToArray();
            }
            else
            {
                return keyeds.
                    Select(x => Alloc(x)).
                    ToArray();
            }
        }

        public bool TryAlloc(TKeyed keyed, out CacheProxy<TKey, TValue> cache)
        {
            cache = keyed == null ? null : Alloc(keyed);
            return cache != null;
        }

        public void Clear()
        {
            using (LockSlim.Scope(LockStatus.Write))
            {
                CacheProxies.Clear();
            }
        }

        public void Reset(IEnumerable<TKey> keys)
        {
            if (keys == null) { return; }

            using (LockSlim.Scope(LockStatus.Read))
            {
                foreach (var cache in CacheProxies.Join(keys, x => x.Key, y => y, (x, y) => x))
                {
                    cache.Reset();
                };
            }
        }

        public void Reset()
        {
            using (LockSlim.Scope(LockStatus.Read))
            {
                foreach (var cache in CacheProxies)
                {
                    cache.Reset();
                }
            }
        }

        private bool Remove(TKey key)
        {
            if (key == null) { return true; }

            using (LockSlim.Scope(LockStatus.Write))
            {
                return CacheProxies.Remove(key);
            }
        }

        private void Remove(IEnumerable<TKey> keys)
        {
            if (keys == null) { return; }

            using (LockSlim.Scope(LockStatus.Write))
            {
                foreach (var key in keys)
                {
                    CacheProxies.Remove(key);
                }
            }
        }

        public bool Remove(TKeyed keyed) => Remove(KeyBuilder.GetKey(keyed));

        public void Remove(IEnumerable<TKeyed> keyeds) => Remove(keyeds.Select(x => KeyBuilder.GetKey(x)));

        public void DoOut()
        {
            using (LockSlim.Scope(LockStatus.Write))
            {
                var items = CacheProxies.Where(x => x.Status == CacheStatus.Real && x.TimedOut == false).ToArray();
                CacheProxies.Clear();
                foreach (var item in items)
                {
                    CacheProxies.Add(item);
                }
            }
        }

        internal void OnDataSourceChanged(object sender, DataSourceChangedEventArgs<TKey> e)
        {
            switch (e.RefreshWith)
            {
                case RefreshCacheWith.Reset:
                    Reset();
                    break;

                case RefreshCacheWith.ResetContains:
                    if (e.Keys?.Length > 0) { Reset(e.Keys); }
                    break;

                case RefreshCacheWith.Clear:
                    Clear();
                    break;

                case RefreshCacheWith.Remove:
                    if (e.Keys?.Length > 0) { Remove(e.Keys); }
                    break;

                case RefreshCacheWith.None:
                default:
                    break;
            }
        }

        public IEnumerable<CacheProxy<TKey, TValue>> AsEnumerable()
        {
            using (LockSlim.Scope(LockStatus.Read))
            {
                return CacheProxies.ToArray();
            }
        }

        public int Count => CacheProxies.Count;
        public IDataSourceMonitoring<TKey> Monitoring { get; internal set; }
        public bool CanMonitoring => Monitoring != null;
        public bool HasMaxCapacity => MaxCapacity > 0;
        public bool HasTimeout => Timeout != TimeSpan.Zero;
        public TimeSpan Timeout { get; internal set; } = TimeSpan.Zero;
        public int MaxCapacity { get; internal set; } = 0;
    }

    /// <summary>
    /// 要素を保持します。
    /// </summary>
    /// <typeparam name="TKey">要素を選別するキーの型</typeparam>
    /// <typeparam name="TValue">要素の型</typeparam>
    public sealed class CacheStore<TKey, TValue> : ICacheStore<TKey, TValue>
    {
        internal InternalCacheStore<TKey, TKey, TValue> Internal { get; }

        internal CacheStore()
        {
            Internal = new InternalCacheStore<TKey, TKey, TValue>
            {
                KeyBuilder = new FuncCacheKeyFactory<TKey, TKey>() { Build = key => key }
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public CacheStore(Func<TKey, TValue> valueBuilder)
        {
            Internal = new InternalCacheStore<TKey, TKey, TValue>(
                new FuncCacheKeyFactory<TKey, TKey>
                {
                    Build = key => key
                },
                new FuncCacheValueBuilder<TKey, TKey, TValue>
                {
                    Build = (_, key) => valueBuilder(key)
                });
        }

        /// <summary>
        /// 
        /// </summary>
        public CacheStore(Func<TKey, TValue> valueBuilder, IDataSourceMonitoring<TKey> monitoring)
        {
            Internal = new InternalCacheStore<TKey, TKey, TValue>(
                new FuncCacheKeyFactory<TKey, TKey>
                {
                    Build = key => key
                },
                new FuncCacheValueBuilder<TKey, TKey, TValue>
                {
                    Build = (_, key) => valueBuilder(key)
                },
                monitoring);
        }

        /// <summary>
        /// 要素の最大保持量を取得します。
        /// </summary>
        public int MaxCapacity { get => Internal.MaxCapacity; internal set => Internal.MaxCapacity = value; }

        /// <summary>
        /// /最大保持量を持っているかどうかを示す値を取得します。
        /// </summary>
        public bool HasMaxCapacity => Internal.HasMaxCapacity;

        /// <summary>
        /// 要素の保持期間を取得します。
        /// </summary>
        public TimeSpan Timeout { get => Internal.Timeout; internal set => Internal.Timeout = value; }

        /// <summary>
        /// 保持期間を持っているかどうかを示す値を取得します。
        /// </summary>
        public bool HasTimeout => Internal.HasTimeout;

        /// <summary>
        /// データソース監視オブジェクトを取得します。
        /// </summary>
        public IDataSourceMonitoring<TKey> Monitoring { get => Internal.Monitoring; internal set => Internal.Monitoring = value; }

        /// <summary>
        /// データソース監視ができるかどうかを示す値を取得します。
        /// </summary>
        public bool CanMonitoring => Internal.CanMonitoring;

        /// <summary>
        /// 要素の一覧を取得します。
        /// </summary>
        public IEnumerable<CacheProxy<TKey, TValue>> AsEnumerable() => Internal.AsEnumerable();

        /// <summary>
        /// 要素の保持数を取得します。
        /// </summary>
        public int Count => Internal.Count;

        /// <summary>
        /// 保持している要素すべて削除します。
        /// </summary>
        public void Clear() => Internal.Clear();

        /// <summary>
        /// <para>保持している要素を圧縮して整理します。</para>
        /// <para>この操作は、実要素を保持していない要素を削除します。</para>
        /// </summary>
        public void DoOut() => Internal.DoOut();

        /// <summary>
        /// すべての要素をリセットします。
        /// </summary>
        public void Reset() => Internal.Reset();

        /// <summary>
        /// 指定したキーで要素を引き当てます。
        /// </summary>
        /// <param name="key">要素を選別するキー</param>
        public CacheProxy<TKey, TValue> Alloc(TKey key) => Internal.Alloc(key);

        /// <summary>
        /// 指定したキーの一覧に一致する要素を引き当てます。
        /// </summary>
        /// <param name="keys">キーの一覧</param>
        public CacheProxy<TKey, TValue>[] Alloc(IEnumerable<TKey> keys) => Internal.Alloc(keys);

        /// <summary>
        /// 指定したキーで要素の引当を試みます。
        /// </summary>
        /// <param name="key">要素を選別するキー</param>
        /// <param name="cache">要素</param>
        public bool TryAlloc(TKey key, out CacheProxy<TKey, TValue> cache) => Internal.TryAlloc(key, out cache);

        /// <summary>
        /// 指定したキーで要素を削除します。
        /// </summary>
        /// <param name="key">要素を識別するキー</param>
        public bool Remove(TKey key) => Internal.Remove(key);

        /// <summary>
        /// 指定したキーの一覧に一致する要素を削除します。
        /// </summary>
        /// <param name="keys">キーの一覧</param>
        public void Remove(IEnumerable<TKey> keys) => Internal.Remove(keys);

        /// <summary>
        /// 指定した要素を登録します。
        /// </summary>
        /// <param name="key">要素のキー</param>
        /// <param name="value">登録する要素</param>
        public CacheProxy<TKey, TValue> Alloc(TKey key, TValue value) => Internal.Alloc(key, value);

    }

    /// <summary>
    /// 要素を保持します。
    /// </summary>
    /// <typeparam name="TKeyed">要素のキーを保有する型</typeparam>
    /// <typeparam name="TKey">要素を選別するキーの型</typeparam>
    /// <typeparam name="TValue">要素の型</typeparam>
    public sealed class CacheStore<TKeyed, TKey, TValue> : ICacheStore<TKeyed, TKey, TValue>
    {
        internal InternalCacheStore<TKeyed, TKey, TValue> Internal { get; }

        internal CacheStore()
        {
            Internal = new InternalCacheStore<TKeyed, TKey, TValue>();
        }

        /// <summary>
        /// 
        /// </summary>
        public CacheStore(Func<TKeyed, TKey> keyBuilde, Func<TKeyed, TKey, TValue> valueBuilder)
        {
            Internal = new InternalCacheStore<TKeyed, TKey, TValue>(
                new FuncCacheKeyFactory<TKeyed, TKey>
                {
                    Build = keyBuilde
                },
                new FuncCacheValueBuilder<TKeyed, TKey, TValue>
                {
                    Build = valueBuilder
                });
        }

        /// <summary>
        /// 
        /// </summary>
        public CacheStore(Func<TKeyed, TKey> keyBuilde, Func<TKeyed, TKey, TValue> valueBuilder, IDataSourceMonitoring<TKey> monitoring)
        {
            Internal = new InternalCacheStore<TKeyed, TKey, TValue>(
                new FuncCacheKeyFactory<TKeyed, TKey>
                {
                    Build = keyBuilde
                },
                new FuncCacheValueBuilder<TKeyed, TKey, TValue>
                {
                    Build = valueBuilder
                },
                monitoring);
        }

        /// <summary>
        /// 要素の最大保持量を取得します。
        /// </summary>
        public int MaxCapacity { get => Internal.MaxCapacity; internal set => Internal.MaxCapacity = value; }

        /// <summary>
        /// /最大保持量を持っているかどうかを示す値を取得します。
        /// </summary>
        public bool HasMaxCapacity => Internal.HasMaxCapacity;

        /// <summary>
        /// 要素の保持期間を取得します。
        /// </summary>
        public TimeSpan Timeout { get => Internal.Timeout; internal set => Internal.Timeout = value; }

        /// <summary>
        /// 保持期間を持っているかどうかを示す値を取得します。
        /// </summary>
        public bool HasTimeout => Internal.HasTimeout;

        /// <summary>
        /// データソース監視オブジェクトを取得します。
        /// </summary>
        public IDataSourceMonitoring<TKey> Monitoring { get => Internal.Monitoring; internal set => Internal.Monitoring = value; }

        /// <summary>
        /// データソース監視ができるかどうかを示す値を取得します。
        /// </summary>
        public bool CanMonitoring => Internal.CanMonitoring;

        /// <summary>
        /// 要素の一覧を取得します。
        /// </summary>
        public IEnumerable<CacheProxy<TKey, TValue>> AsEnumerable() => Internal.AsEnumerable();

        /// <summary>
        /// 要素の保持数を取得します。
        /// </summary>
        public int Count => Internal.Count;

        /// <summary>
        /// 保持している要素すべて削除します。
        /// </summary>
        public void Clear() => Internal.Clear();

        /// <summary>
        /// <para>保持している要素を圧縮して整理します。</para>
        /// <para>この操作は、実要素を保持していない要素を削除します。</para>
        /// </summary>
        public void DoOut() => Internal.DoOut();

        /// <summary>
        /// すべての要素をリセットします。
        /// </summary>
        public void Reset() => Internal.Reset();

        /// <summary>
        /// 指定したオブジェクトから要素を引き当てます。
        /// </summary>
        /// <param name="keyed">キーを保有するオブジェクト</param>
        public CacheProxy<TKey, TValue> Alloc(TKeyed keyed) => Internal.Alloc(keyed);

        /// <summary>
        /// 指定したオブジェクトの一覧から要素を引き当てます。
        /// </summary>
        /// <param name="keyeds">キーを保有するオブジェクトの一覧</param>
        public CacheProxy<TKey, TValue>[] Alloc(IEnumerable<TKeyed> keyeds) => Internal.Alloc(keyeds);

        /// <summary>
        /// 指定したオブジェクトで要素の引当を試みます。
        /// </summary>
        /// <param name="keyed">キーを保有するオブジェクト</param>
        /// <param name="cache">要素</param>
        public bool TryAlloc(TKeyed keyed, out CacheProxy<TKey, TValue> cache) => Internal.TryAlloc(keyed, out cache);

        /// <summary>
        /// 指定したオブジェクトで要素を削除します。
        /// </summary>
        /// <param name="keyed">キーを保有するオブジェクト</param>
        public bool Remove(TKeyed keyed) => Internal.Remove(keyed);

        /// <summary>
        /// 指定したオブジェクトの一覧から要素を削除します。
        /// </summary>
        /// <param name="keyeds">キーを保有するオブジェクトの一覧</param>
        public void Remove(IEnumerable<TKeyed> keyeds) => Internal.Remove(keyeds);

        /// <summary>
        /// 指定した要素を登録します。
        /// </summary>
        /// <param name="keyed">キーを保有するオブジェクト</param>
        /// <param name="value">登録する要素</param>
        public CacheProxy<TKey, TValue> Alloc(TKeyed keyed, TValue value) => Internal.Alloc(keyed, value);
    }
}