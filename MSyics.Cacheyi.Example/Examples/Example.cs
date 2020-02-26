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

        public abstract void ShowCore();

        public virtual void Teardown()
        {
            Traceable.Shutdown();
        }

        public virtual Task ShowAsync() { return Task.CompletedTask; }
    }
}
