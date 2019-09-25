using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class ManualAllocExample : Example
    {
        public override string Name => nameof(ManualAllocExample);

        public override void ShowCore()
        {
            var cache = new HogeCacheCenter();
            {
                // insert
                cache.HogeStore.Alloc(1, new Hoge { Id = 1, Message = "hogehoge" });
                var proxy = cache.HogeStore.Alloc(1);
                Tracer.Information(proxy.GetValue().Message);
            }
            {
                var proxy = cache.HogeStore.Alloc(2);
                Tracer.Information(proxy.GetValue().Message);
            }
            {
                // update
                cache.HogeStore.Alloc(2, new Hoge { Id = 2, Message = "piyopiyo" });
                var proxy = cache.HogeStore.Alloc(2);
                Tracer.Information(proxy.GetValue().Message);
            }
        }

        public class HogeCacheCenter : CacheCenter
        {
            public CacheStore<int, Hoge> HogeStore { get; private set; }

            protected override void ConstructStore(CacheStoreDirector director)
            {
                director.Build(() => HogeStore)
                        .Settings(settings =>
                        {
                            settings.MaxCapacity = 100;
                            settings.Timeout = TimeSpan.FromMilliseconds(200);
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
