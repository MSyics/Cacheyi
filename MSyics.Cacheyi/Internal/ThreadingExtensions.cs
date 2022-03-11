namespace MSyics.Cacheyi;

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
        TaskCompletionSource<T> tcs = new();
        Timer timer = new(x =>
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
        }, null, timeout, Timeout.InfiniteTimeSpan);
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
        ReaderWriterLockSlimScope scope = new(lockSlim, status);
        scope.Enter();
        return scope;
    }
}

internal record struct ReaderWriterLockSlimScope(ReaderWriterLockSlim LockSlim, LockStatus Status) : IDisposable
{
    public void Enter()
    {
        if (LockSlim is null) { return; }

        switch (Status)
        {
            case LockStatus.UpgradeableRead:
                LockSlim.EnterUpgradeableReadLock();
                break;
            case LockStatus.Write:
                LockSlim.EnterWriteLock();
                break;
            case LockStatus.Read:
                LockSlim.EnterReadLock();
                break;
            case LockStatus.None:
            default:
                break;
        }
    }

    public void Exit()
    {
        if (LockSlim is null) { return; }

        switch (Status)
        {
            case LockStatus.UpgradeableRead:
                LockSlim.ExitUpgradeableReadLock();
                break;
            case LockStatus.Write:
                LockSlim.ExitWriteLock();
                break;
            case LockStatus.Read:
                LockSlim.ExitReadLock();
                break;
            case LockStatus.None:
            default:
                break;
        }

        LockSlim = null;
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
