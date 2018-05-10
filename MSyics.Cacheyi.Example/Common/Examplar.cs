using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Cacheyi
{
    abstract class Examplar
    {
        static Examplar()
        {
            Traceable.Add("logging.json");
        }

        protected Tracer Tracer { get; } = Traceable.Get();
        private List<Example> Examples { get; } = new List<Example>();

        public Examplar Add<T>() where T : Example, new()
        {
            Examples.Add(new T());
            return this;
        }

        public void Test()
        {
            foreach (var item in Examples)
            {
                using (Tracer.Scope())
                {
                    item.Test();
                }
            }

            Traceable.Shutdown();
        }
    }
}
