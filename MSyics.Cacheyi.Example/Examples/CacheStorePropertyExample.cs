using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class CacheStorePropertyExample : Example
    {
        public override string Name => nameof(CacheStorePropertyExample);

        public override void ShowCore()
        {
            {
                var cache = new HogeHoge();
                Tracer.Information(cache.Hoge.Message);
                {
                    var proxy = cache.HogeStore.Alloc(1, new Hoge { Id = 1, Message = "hogehoge" });
                    Tracer.Information(proxy.GetValue().Message);
                }
                {
                    var proxy = cache.HogeStore.Alloc(1);
                    Tracer.Information(proxy.GetValue().Message);
                }
                {
                    var proxy = cache.HogeStore.Alloc(2);
                    Tracer.Information(proxy.GetValue().Message);
                }
            }
            {
                var cache = new HogeHoge();
                Tracer.Information(cache.Hoge.Message);
                {
                    var proxy = cache.HogeStore.Alloc(1);
                    Tracer.Information(proxy.GetValue().Message);
                }
                {
                    var proxy = cache.HogeStore.Alloc(2);
                    Tracer.Information(proxy.GetValue().Message);
                }
            }
        }

        public class HogeHoge
        {
            public HogeHoge()
            {
                CacheCenter.ConstructStore(this, director =>
                {
                    director.
                    Build(() => HogeStore).
                    GetValue(key => new Hoge
                    {
                        Id = key,
                        Message = $"{DateTime.Now:ffff}",
                    });
                });

                Hoge = new Hoge { Message = DateTime.Now.Ticks.ToString() };
            }

            public Hoge Hoge { get; set; }
            public ICacheStore<int, Hoge> HogeStore { get; private set; }
        }

        public class Hoge
        {
            public int Id { get; set; }
            public string Message { get; set; }
        }
    }
}
