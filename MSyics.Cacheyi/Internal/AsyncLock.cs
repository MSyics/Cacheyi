namespace MSyics.Cacheyi;

internal sealed class AsyncLock
{
    readonly SemaphoreSlim semaphore = new(1, 1);
    readonly IDisposable releaser;

    public AsyncLock()
    {
        releaser = new Releasable(semaphore);
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

    private class Releasable : IDisposable
    {
        readonly SemaphoreSlim semaphore;
        public Releasable(SemaphoreSlim semaphore) { this.semaphore = semaphore; }
        public void Dispose() { semaphore.Release(); }
    }
}
