/****************************************************************
© 2017 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// <para>キャッシュされるオブジェクトを関連付けます。</para>
    /// <para>このクラスはオブジェクトを取得するためのプロキシーです。</para>
    /// </summary>
    /// <typeparam name="TKey">キー項目の型</typeparam>
    /// <typeparam name="TValue">キャッシュされるオブジェクトの型</typeparam>
    public sealed class CacheProxy<TKey, TValue>
    {
        private object m_thisLock = new object();

        internal CacheProxy() { }

        /// <summary>
        /// オブジェクトを取得します。
        /// </summary>
        /// <returns>オブジェクト</returns>
        public TValue Get()
        {
            lock (m_thisLock)
            {
                if (Status == CacheStatus.Virtual)
                {
                    CacheValue = ValueFactoryCallBack();
                    Status = CacheStatus.Real;

                    if (HasTimeout)
                    {
                        CancellingTimeout = TimedOutCallBack.StartNewTimer(Timeout);
                    }
                }
                return CacheValue.Value;
            }
        }

        /// <summary>
        /// オブジェクトとの関連を Virtual にします。
        /// </summary>
        public CacheProxy<TKey, TValue> Reset()
        {
            lock (m_thisLock)
            {
                Status = CacheStatus.Virtual;
                CancellingTimeout?.TrySetCanceled();
                return this;
            }
        }

        /// <summary>
        /// オブジェクトとの関連状態を取得します。
        /// </summary>
        public CacheStatus Status { get; private set; } = CacheStatus.Virtual;

        /// <summary>
        /// オブジェクトの保持期間を経過したかどうか示す値を取得します。
        /// </summary>
        public bool TimedOut
        {
            get
            {
                if (Status != CacheStatus.Real) { return false; }
                return HasTimeout ? DateTimeOffset.Now >= CacheValue.Created.Add(Timeout) : false;
            }
        }

        /// <summary>
        /// オブジェクトの保持期間を取得します。
        /// </summary>
        public TimeSpan Timeout { get; internal set; } = TimeSpan.Zero;

        /// <summary>
        /// タイムアウトするかどうかを示す値を取得します。
        /// </summary>
        public bool HasTimeout => Timeout != TimeSpan.Zero;

        /// <summary>
        /// キャッシュオブジェクトのキーを取得します。
        /// </summary>
        public TKey Key => CacheKey;

        internal TKey CacheKey { get; set; }
        internal CacheValue<TValue> CacheValue { get; set; }
        internal Func<CacheValue<TValue>> ValueFactoryCallBack { get; set; }
        internal Func<bool> TimedOutCallBack { get; set; }
        internal TaskCompletionSource<bool> CancellingTimeout { get; set; }
    }
}
