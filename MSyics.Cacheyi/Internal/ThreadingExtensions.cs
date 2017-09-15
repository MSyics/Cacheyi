/****************************************************************
© 2017 MSyics
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
                if (tcs.Task.IsCompleted) return;
                try
                {
                    var value = func();
                    tcs.TrySetResult(value);
                }
                catch (Exception ex)
                {
                    tcs.TrySetException(ex);
                }
            }, null, timeout, TimeSpan.FromTicks(Timeout.Infinite));
            tcs.Task.ContinueWith(t => timer.Dispose());
            return tcs;
        }
    }
    #endregion

    #region ReaderWriterLockSlimScope
    internal static partial class ThreadingExtensions
    {
        public static ReaderWriterLockSlimScope Scope(this ReaderWriterLockSlim lockAdaptee, LockStatus status) => new ReaderWriterLockSlimScope(lockAdaptee, status);

        internal class ReaderWriterLockSlimScope : IDisposable
        {
            public ReaderWriterLockSlimScope(ReaderWriterLockSlim adaptee, LockStatus status)
            {
                m_adaptee = adaptee;
                m_status = status;

                switch (status)
                {
                    case LockStatus.UpgradeableRead:
                        m_adaptee.EnterUpgradeableReadLock();
                        break;

                    case LockStatus.Write:
                        m_adaptee.EnterWriteLock();
                        break;

                    case LockStatus.Read:
                        m_adaptee.EnterReadLock();
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
                switch (m_status)
                {
                    case LockStatus.UpgradeableRead:
                        m_adaptee.ExitUpgradeableReadLock();
                        break;
                    case LockStatus.Write:
                        m_adaptee.ExitWriteLock();
                        break;
                    case LockStatus.Read:
                        m_adaptee.ExitReadLock();
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
