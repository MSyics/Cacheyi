using MSyics.Traceyi;

abstract class Example : IExample
{
    public abstract string Name { get; }

    protected Tracer Tracer { get; private set; }

    public virtual void Setup()
    {
        Traceable.Add("Traceyi.json");
        Tracer = Traceable.Get();
    }

    void IExample.Show() { }

    async Task IExample.ShowAsync()
    {
        using (Tracer.Scope(label: Name))
        {
            await ShowAsync();
        }
    }

    public abstract Task ShowAsync();

    public virtual void Teardown()
    {
        Traceable.Shutdown();
    }
}