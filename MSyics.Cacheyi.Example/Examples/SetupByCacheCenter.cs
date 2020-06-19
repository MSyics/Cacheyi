using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class SetupByCacheCenter : Example
    {
        public override string Name => nameof(SetupByCacheCenter);

        public override async Task ShowAsync()
        {
            for (int i = 0; i < 2; i++)
            {
                using (Tracer.Scope())
                {
                    await Fire();
                }
            }
        }

        private async Task Fire()
        {
            var cache = new ProductCenter();

            Tracer.Information(cache.Products.Allocate(1).GetValue());

            Tracer.Debug("wait");
            await Task.Delay(100);
            Tracer.Information(cache.Products.Allocate(2).GetValue());

            Tracer.Debug("wait");
            await Task.Delay(100);
            Tracer.Information(cache.Products.Allocate(1).GetValue());
        }

        #region Product
        class ProductCenter : CacheCenter
        {
            public CacheStore<int, Product> Products { get; private set; }

            protected override void ConstructStore(CacheStoreDirector director)
            {
                director.
                    Build(() => Products).
                    Settings(settings =>
                    {
                        settings.MaxCapacity = 100;
                    }).
                    GetValue(key => new Product
                    {
                        Id = key,
                        Message = $"{key:000}",
                        Timestamp = DateTime.Now,
                    });
            }
        }


        class Product
        {
            public int Id { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }

            public override string ToString()
            {
                return $"{Id},{Message},{Timestamp.TimeOfDay}";
            }
        }
        #endregion
    }
}
