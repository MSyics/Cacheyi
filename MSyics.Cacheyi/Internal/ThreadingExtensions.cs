using System;
using System.Threading;
using System.Threading.Tasks;

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
        public static IDisposable Scope(this ReaderWriterLockSlim lockSlim, LockStatus status)
        {
            var scope = new ReaderWriterLockSlimScope()
            {
                lockSlim = lockSlim,
                Status = status,
            };
            scope.Enter();
            return scope;
        }
    }

    internal class ReaderWriterLockSlimScope : IDisposable
    {
        public ReaderWriterLockSlim lockSlim { get; set; }
        public LockStatus Status { get; set; } = LockStatus.None;

        public void Enter()
        {
            switch (Status)
            {
                case LockStatus.UpgradeableRead:
                    lockSlim.EnterUpgradeableReadLock();
                    break;
                case LockStatus.Write:
                    lockSlim.EnterWriteLock();
                    break;
                case LockStatus.Read:
                    lockSlim.EnterReadLock();
                    break;
                case LockStatus.None:
                default:
                    break;
            }
        }

        public void Exit()
        {
            switch (Status)
            {
                case LockStatus.UpgradeableRead:
                    lockSlim.ExitUpgradeableReadLock();
                    break;
                case LockStatus.Write:
                    lockSlim.ExitWriteLock();
                    break;
                case LockStatus.Read:
                    lockSlim.ExitReadLock();
                    break;
                case LockStatus.None:
                default:
                    break;
            }
        }

        public void Dispose() => Exit();
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
