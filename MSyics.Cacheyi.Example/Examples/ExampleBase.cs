using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    abstract class Example : IExample
    {
        public abstract string Name { get; }

        protected Tracer Tracer = Traceable.Get();

        static Example()
        {
            Traceable.Add("Traceyi.json");
        }

        public virtual void Setup() { }

        public void Show()
        {
            using (Tracer.Scope(Name))
            {
                ShowCore();
            }
        }

        public virtual void ShowCore() { }
        public virtual Task ShowCoreAsync() => Task.CompletedTask;

        public virtual void Teardown()
        {
            Traceable.Shutdown();
        }

        public async Task ShowAsync()
        {
            var scope = Tracer.Scope();
            try
            {
                await ShowCoreAsync();
            }
            finally
            {
                scope.Dispose();
            }
        }
    }
}
