using MSyics.Traceyi;
using System.Threading.Tasks;

namespace MSyics.Cacheyi
{
    internal abstract class Example
    {
        protected Tracer Tracer { get; } = Traceable.Get();
        public abstract string Name { get; }
        public abstract void Test();
    }
}
