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
            {
                var cache = new HogeHoge();
                {
                    var proxy = cache.HogeStore.Alloc(1, new Hoge { Id = 1, Message = "hogehoge" });
                    Tracer.Information(proxy.Status);
                    Tracer.Information(proxy.GetValue().Message);
                }
                {
                    var proxy = cache.HogeStore.Alloc(1);
                    Tracer.Information(proxy.Status);
                    Tracer.Information(proxy.GetValue().Message);
                }
                {
                    cache.HogeStore.Alloc(1, new Hoge { Id = 1, Message = "piyopiyo" });
                }
                {
                    var proxy = cache.HogeStore.Alloc(1);
                    Tracer.Information(proxy.Status);
                    Tracer.Information(proxy.GetValue().Message);
                }
            }
            {
                var cache = new HogeHoge();
                {
                    var proxy = cache.HogeStore.Alloc(1);
                    Tracer.Information(proxy.Status);
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
                    GetValue(key =>
                    {
                        return new Hoge
                        {
                            Id = key,
                            Message = $"{DateTime.Now:ffff}",
                        };
                    });
                });

                HogeStore = new CacheStore<int, Hoge>(key =>
                {
                    return new Hoge
                    {
                        Id = key,
                        Message = $"{DateTime.Now:ffff}",
                    };
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
