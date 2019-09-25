using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class HogeExample : Example
    {
        public override string Name => nameof(HogeExample);

        public override void ShowCore()
        {
            var cache = new HogeCacheCenter();

            Fire();
        }

        public void Fire()
        {
            var items = new List<string>();
            var cache = new HogeCacheCenter();

            for (int _ = 0; _ < 10; _++)
            {
                items.Clear();
                cache.HogeStore.Clear();
                {
                    Parallel.For(0, 100000, i =>
                    {
                        var value = cache.HogeStore.Alloc(1).GetValue();
                        lock (this)
                        {
                            items.Add(value.Message);
                        }
                    });

                    foreach (var item in items.ToLookup(x => x ?? ""))
                    {
                        Tracer.Information($"{item.Key}:{item.Count()}");
                    }
                }
            }

            cache.HogeStore.Clear();
            Parallel.For(0, 10000, i =>
            {
                cache.HogeStore.Alloc(i).GetValue();
            });

            //cache.HogeStore.Reset();
            //foreach (var item in cache.HogeStore.Where(x => x.TimedOut))
            foreach (var item in cache.HogeStore.AsEnumerable().Where(x => !x.TimedOut))
            {
                Tracer.Information($"{item.Key} {item.TimedOut}");
            }
            Tracer.Information($"{cache.HogeStore.Count}");

        }


        public class HogeCacheCenter : CacheCenter
        {
            public CacheStore<int, Hoge> HogeStore { get; private set; }

            protected override void ConstructStore(CacheStoreDirector director)
            {
                director.Build(() => HogeStore)
                        .Settings(settings =>
                        {
                            settings.MaxCapacity = int.MaxValue;
                            settings.Timeout = TimeSpan.FromMilliseconds(10);
                        })
                        .GetValue(key => new Hoge
                        {
                            Id = key,
                            Message = $"{DateTime.Now:ffff}",
                        });
            }
        }

        public class Hoge
        {
            public int Id { get; set; }
            public string Message { get; set; }
        }
    }
}
