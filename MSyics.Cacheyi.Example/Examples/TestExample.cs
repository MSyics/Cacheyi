using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class TestExample : Example
    {
        public override string Name => nameof(TestExample);

        public override void ShowCore()
        {
            for (int i = 0; i < 50; i++)
            {
                Task.WhenAll(
                    AddCache(),
                    ReadCache()
                    ).Wait();
            }
        }

        public async Task AddCache()
        {
            var cache = new HogeHoge();
            await Task.Delay(1);
            for (int i = 0; i < 100; i++)
            {
                //await Task.Delay(1);
                await Task.Yield();
                var p = cache.HogeStore.Alloc(i);
                p.Reset();
                //Tracer.Information(p.Key);
            }
            Tracer.Information("end AddCache");
        }

        public async Task ReadCache()
        {
            var cache = new HogeHoge();
            await Task.Delay(1);
            //await Task.Yield();
            foreach (var item in cache.HogeStore)
            {
                //await Task.Delay(1);
                await Task.Yield();
                Tracer.Information($"{item.Key} {item.GetValue().Message}");
            }
            Tracer.Information("end ReadCache");
        }

        public class HogeHoge
        {
            public HogeHoge()
            {
                CacheCenter.ConstructStore(this, director =>
                {
                    director.
                    Build(() => HogeStore).
                    GetValue(key =>
                    {
                        Thread.Sleep(100);
                        return new Hoge
                        {
                            Id = key,
                            Message = $"{DateTime.Now:ffff}",
                        };
                    });
                });
            }

            public ICacheStore<int, Hoge> HogeStore { get; private set; }
        }

        public class Hoge
        {
            public int Id { get; set; }
            public string Message { get; set; }
        }
    }
}
