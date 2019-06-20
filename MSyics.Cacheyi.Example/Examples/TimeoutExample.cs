using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class TimeoutExample : Example
    {
        public override string Name => nameof(TimeoutExample);

        public override async Task ShowCoreAsync()
        {
            var cache = new TimeoutCacheCenter();

            Tracer.Information("hogehoge");

            await Task.WhenAll(
                Enumerable
                .Range(0, 9)
                .Select(i =>
                {
                    return Task.Run(() =>
                    {
                        cache
                        .TimeoutElements
                        .Alloc(Enumerable.Range(i * 1000, 1000 - 1))
                        .ToList()
                        .ForEach(x => x.GetValue());
                    });
                }).ToArray());


            Tracer.Information("hogehoge");


            await Task.Run(async () =>
            {
                while (cache.TimeoutElements.Count > 0)
                {
                    await Task.Delay(333);
                    Tracer.Information(cache.TimeoutElements.Count);
                }
            });

        }
    }

    public class TimeoutCacheCenter : CacheCenter
    {
        public CacheStore<int, TimeoutElement> TimeoutElements { get; private set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => TimeoutElements)
                    .Settings(settings =>
                    {
                        //settings.MaxCapacity = 100;
                        settings.Timeout = new TimeSpan(0, 0, 1);
                    })
                    .GetValue((key) => new TimeoutElement { Id = key, Message = $"{DateTime.Now:ffff}" });
        }
    }

    public class TimeoutElement
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }
}
