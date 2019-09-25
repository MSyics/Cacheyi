using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class DoOutExample : Example
    {
        public override string Name => nameof(DoOutExample);

        public override void ShowCore()
        {
            var cache = new HogeCacheCenter();

            foreach (var item in Enumerable.Range(1, 10000))
            {
                var proxy = cache.HogeStore.Alloc(item);
                proxy.GetValue();
            }

            Tracer.Information(cache.HogeStore.Count);
            cache.HogeStore.Reset();
            cache.HogeStore.DoOut();
            Tracer.Information(cache.HogeStore.Count);
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
                            //settings.Timeout = TimeSpan.FromMilliseconds(2000);
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
