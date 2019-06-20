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
            var proxy = cache.Elements.Alloc(1);
            Console.WriteLine(proxy.GetValue());

            await Task.Delay(2000);

            proxy = cache.Elements.Add(1, DateTime.Now);
            Console.WriteLine(proxy.GetValue());

            proxy.Reset();
            Console.WriteLine(proxy.GetValue());


            await Task.CompletedTask;
        }
    }

    class AddCacheCenter : CacheCenter
    {
        public CacheStore<int, DateTime> Elements { get; set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => Elements)
                    .GetValue(id => DateTime.Now);
        }
    }
}
