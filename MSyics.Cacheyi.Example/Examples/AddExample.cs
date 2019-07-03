using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class AddExample : Example
    {
        public override string Name => nameof(AddExample);

        public override async Task ShowCoreAsync()
        {
            var cache = new AddCacheCenter();

            var key = 1;
            {
                // Alloc DateTime
                cache.DateTimes.Alloc(key);
                Tracer.Information(cache.DateTimes.Alloc(key).GetValue());
                await Task.Delay(1000);
                Tracer.Information(cache.DateTimes.Alloc(key).GetValue());
            }
            {
                // Overwrite DateTime
                cache.DateTimes.Alloc(key, DateTime.MinValue);
                Tracer.Information(cache.DateTimes.Alloc(key).GetValue());
                await Task.Delay(1000);
                Tracer.Information(cache.DateTimes.Alloc(key).GetValue());
            }

            var keyed = (1, "hogehoge");
            {
                // Alloc Message
                var p = cache.Messages.Alloc(keyed);
                Tracer.Information(cache.Messages.Alloc(keyed).GetValue());
                await Task.Delay(1000);
                Tracer.Information(cache.Messages.Alloc(keyed).GetValue());
            }
            {
                // Overwrite Message
                cache.Messages.Alloc(keyed, "piyopiyo");
                Tracer.Information(cache.Messages.Alloc(keyed).GetValue());
                await Task.Delay(1000);
                Tracer.Information(cache.Messages.Alloc(keyed).GetValue());
                Tracer.Information(cache.Messages.Alloc(keyed).Reset().GetValue());

            }
        }
    }

    class AddCacheCenter : CacheCenter
    {
        public CacheStore<int, DateTime> DateTimes { get; set; }
        public CacheStore<(int id, string message), int, string> Messages { get; set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => DateTimes)
                    .GetValue(id => DateTime.Now);

            director.Build(() => Messages)
                    .GetKey(x => x.id)
                    .GetValue((x, key) => x.message);
        }
    }
}
