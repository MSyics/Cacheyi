using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Example
{
    class SetupExample : IExample
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

        public void Test()
        {
            var tracer = Traceable.Get();

            tracer.Debug(Cache.DateTimes[0].GetValue());
            Task.Delay(100).Wait();
            tracer.Debug(Cache.DateTimes[0].GetValue());

            Cache.DateTimes[0].Reset();
            tracer.Debug(Cache.DateTimes[0].GetValue());
        }
    }
}
