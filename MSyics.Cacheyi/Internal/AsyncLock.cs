namespace MSyics.Cacheyi;

internal sealed class AsyncLock
{
    readonly SemaphoreSlim semaphore = new(1, 1);
    readonly IDisposable releaser;

    public AsyncLock()
    {
        releaser = new Releasable(this);
    }

    public async ValueTask<IDisposable> LockAsync(CancellationToken cancellationToken = default)
    {
        var wait = semaphore.WaitAsync(cancellationToken);

        if (wait.IsCompleted)
        {
            return releaser;
        }
        else
        {
            return await wait.
                ContinueWith(
                    (_, state) => (IDisposable)state!,
                    releaser,
                    CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default).
                ConfigureAwait(false);
        }
    }

    private struct Releasable : IDisposable
    {
        readonly AsyncLock target;
        public Releasable(AsyncLock target) { this.target = target; }
        public void Dispose() { target.semaphore.Release(); }
    }
}
