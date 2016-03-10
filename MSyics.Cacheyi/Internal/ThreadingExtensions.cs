/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Threading.Tasks;
using System.Threading;

namespace MSyics.Cacheyi
{
    #region TaskCompletionSource
    internal static partial class ThreadingExtensions
    {
        /// <summary>
        /// デリゲートを指定した遅延時間で実行します。
        /// </summary>
        /// <typeparam name="T">結果の型</typeparam>
        /// <param name="func">遅延実行するデリゲート</param>
        /// <param name="timeout">遅延時間</param>
        public static TaskCompletionSource<T> StartNewTimer<T>(this Func<T> func, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<T>();
            var timer = new Timer(x =>
            {
                if (!(tcs.Task.IsCompleted))
                {
                    try
                    {
                        var value = func();
                        tcs.TrySetResult(value);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, null, timeout, TimeSpan.FromTicks(Timeout.Infinite));

            tcs.Task.ContinueWith(t => timer.Dispose());

            return tcs;
        }

        private delegate void TimerCallback(object state);
        private sealed class Timer : CancellationTokenSource
        {
            public Timer(TimerCallback callback, object state, TimeSpan dueTime, TimeSpan period)
            {
                Task.Delay(dueTime, Token).ContinueWith(async (t, s) =>
                {
                    var tuple = (Tuple<TimerCallback, object>)s;

                    while (true)
                    {
                        if (IsCancellationRequested)
                            break;
                        await Task.Run(() => tuple.Item1(tuple.Item2));
                        await Task.Delay(period);
                    }

                }, Tuple.Create(callback, state), CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion,
                    TaskScheduler.Default);
            }

            public void Stop() { base.Cancel(); }
        }
    }
    #endregion

    #region ReaderWriterLockSlimScope

    internal static partial class ThreadingExtensions
    {
        public static ReaderWriterLockSlimScope Scope(this ReaderWriterLockSlim lockAdaptee, LockStatus status)
        {
            return new ReaderWriterLockSlimScope(lockAdaptee, status);
        }

        internal class ReaderWriterLockSlimScope : IDisposable
        {
            public ReaderWriterLockSlimScope(ReaderWriterLockSlim adaptee, LockStatus status)
            {
                this.m_adaptee = adaptee;
                this.m_status = status;

                switch (status)
                {
                    case LockStatus.UpgradeableRead:
                        this.m_adaptee.EnterUpgradeableReadLock();
                        break;
                    case LockStatus.Write:
                        this.m_adaptee.EnterWriteLock();
                        break;
                    case LockStatus.Read:
                        this.m_adaptee.EnterReadLock();
                        break;
                    case LockStatus.None:
                    default:
                        break;
                }
            }

            private ReaderWriterLockSlim m_adaptee;
            private LockStatus m_status = LockStatus.None;

            #region IDisposable Members
            void IDisposable.Dispose()
            {
                switch (this.m_status)
                {
                    case LockStatus.UpgradeableRead:
                        this.m_adaptee.ExitUpgradeableReadLock();
                        break;
                    case LockStatus.Write:
                        this.m_adaptee.ExitWriteLock();
                        break;
                    case LockStatus.Read:
                        this.m_adaptee.ExitReadLock();
                        break;
                    case LockStatus.None:
                    default:
                        break;
                }
            }
            #endregion
        }
    }

    internal enum LockStatus
    {
        None,
        UpgradeableRead,
        Write,
        Read,
    }
    #endregion
}
