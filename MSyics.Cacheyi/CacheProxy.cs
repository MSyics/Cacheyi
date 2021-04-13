using System;
using System.Threading.Tasks;

namespace MSyics.Cacheyi
{
    /// <summary>
    /// 要素を取得するためのプロキシー
    /// </summary>
    /// <typeparam name="TKey">キーの型</typeparam>
    /// <typeparam name="TValue">保持要素の型</typeparam>
    public sealed class CacheProxy<TKey, TValue>
    {
        private readonly object lockObj = new();

        internal CacheProxy() { }

        /// <summary>
        /// 要素を取得します。
        /// </summary>
        public TValue GetValue()
        {
            lock (lockObj)
            {
                if (Status == CacheStatus.Virtual)
                {
                    CacheValue = GetValueCallBack();
                    Status = CacheStatus.Real;

                    if (HasTimeout)
                    {
                        CancellingTimeout = TimedOutCallBack?.StartNewTimer(Timeout);
                    }
                }
                return CacheValue.Value;
            }
        }

        /// <summary>
        /// 要素の保持状態をリセットします。
        /// </summary>
        public CacheProxy<TKey, TValue> Reset()
        {
            lock (lockObj)
            {
                Status = CacheStatus.Virtual;
                CancellingTimeout?.TrySetCanceled();
                return this;
            }
        }

        /// <summary>
        /// 要素の保持状態を保持期間を過ぎていたらリセットします。
        /// </summary>
        public CacheProxy<TKey, TValue> ResetIfTimeout()
        {
            lock (lockObj)
            {
                if (TimedOut)
                {
                    Status = CacheStatus.Virtual;
                    CancellingTimeout?.TrySetCanceled();
                }
                return this;
            }
        }

        /// <summary>
        /// 要素の保持状態を取得します。
        /// </summary>
        public CacheStatus Status { get; private set; } = CacheStatus.Virtual;

        /// <summary>
        /// 要素の保持期間を経過したかどうか示す値を取得します。
        /// </summary>
        public bool TimedOut
        {
            get
            {
                if (Status != CacheStatus.Real) { return false; }
                return HasTimeout && DateTimeOffset.Now >= CacheValue.Cached.Add(Timeout);
            }
        }

        /// <summary>
        /// 要素の保持期間を取得します。
        /// </summary>
        public TimeSpan Timeout { get; internal set; } = TimeSpan.Zero;

        /// <summary>
        /// 要素の保持期間を持つかどうかを示す値を取得します。
        /// </summary>
        public bool HasTimeout => Timeout != TimeSpan.Zero;

        /// <summary>
        /// 要素のキーを取得します。
        /// </summary>
        public TKey Key { get; internal set; }

        internal CacheValue<TValue> CacheValue { get; set; }
        internal Func<CacheValue<TValue>> GetValueCallBack { get; set; }
        internal Func<bool> TimedOutCallBack { get; set; }
        internal TaskCompletionSource<bool> CancellingTimeout { get; set; }
    }
}
