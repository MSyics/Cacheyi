using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi
{
    class _Example : Example
    {
        class ExampleCacheCenter : CacheCenter
        {
            public CacheStore<int, string> DateTimes { get; set; }

            protected override void ConstructStore(CacheStoreDirector director)
            {
                director.Build(() => DateTimes)
                        .GetValue(x => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffffff"));
            }
        }

        ExampleCacheCenter Cache { get; set; } = new ExampleCacheCenter();

        public override void Test()
        {
            Enumerable.Range(0, 100000).Select(i => Cache.DateTimes.Alloc(i)).AsParallel().ToArray();

            Tracer.Information(Cache.DateTimes.Alloc(10).GetValue());
        }
    }
}
