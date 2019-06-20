using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class SetupExample : Example
    {
        public override string Name => nameof(SetupExample);

        public override Task ShowCoreAsync()
        {
            return Task.Run(() =>
            {
                var cache = new HogeCacheCenter();
                {
                    var proxy = cache.HogeStore.Alloc(1);
                    Tracer.Information(proxy.GetValue().Message);
                }
                {
                    var proxy = cache.HogeStore.Alloc(2);
                    Tracer.Information(proxy.GetValue().Message);
                }
                {
                    var proxy = cache.HogeStore.Alloc(1);
                    Tracer.Information(proxy.GetValue().Message);
                }
            });
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
                    })
                    .GetValue((key) => new Hoge { Id = key, Message = $"{DateTime.Now:ffff}" });
        }
    }

    public class Hoge
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}
