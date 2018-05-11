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
            public CacheStore<(int id, string message), int, string> Messages { get; set; }

            protected override void ConstructStore(CacheStoreDirector director)
            {
                director.Build(() => DateTimes)
                        .GetValue(x => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffffff"));

                director.Build(() => Messages)
                        .GetKey(x => x.id)
                        .GetValue((keyed, key) =>
                        {
                            return $"{keyed.message} {DateTime.Now:yyyy/MM/dd HH:mm:ss.fffffff}";
                        });

            }
        }

        ExampleCacheCenter Cache { get; set; } = new ExampleCacheCenter();

        public override string Name => nameof(_Example);

        public override void Test()
        {
            Enumerable.Range(0, 100000).Select(i => Cache.DateTimes.Alloc(i)).AsParallel().ToArray();
            Tracer.Information(Cache.DateTimes.Alloc(10).GetValue());

            var keyeds = new List<(int id, string message)>
            {
                (1, "hogehoge"),
                (2, "piyopiyo"),
                (3, "fugafuga"),
            };

            Tracer.Information(Cache.Messages.Alloc(keyeds[0]).GetValue());
            Thread.Sleep(100);
            Tracer.Information(Cache.Messages.Alloc(keyeds[0]).GetValue());
            Tracer.Information(Cache.Messages.Alloc(keyeds[0]).Reset().GetValue());

            Cache.Messages.DoOut();
            Cache.Messages.Reset();
            Cache.Messages.Clear();
        }
    }
}
