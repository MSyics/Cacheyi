using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi
{
    class SetupExample : Example
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

        public override string Name => nameof(SetupExample);

        public override void Test()
        {
            Tracer.Information(Cache.DateTimes.Alloc(0).GetValue());
        }
    }
}
