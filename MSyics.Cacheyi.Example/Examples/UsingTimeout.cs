using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class UsingTimeout : Example
    {
        public override string Name => nameof(UsingTimeout);

        public override async Task ShowAsync()
        {
            for (int i = 0; i < 2; i++)
            {
                using (Tracer.Scope(label: i))
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
        public class ProductCenter
        {
            public ProductCenter()
            {
                CacheCenter.ConstructStore(this, director =>
                {
                    CacheStoreDirector.Build(() => Products).
                    Settings(settings =>
                    {
                        settings.Timeout = TimeSpan.FromMilliseconds(200);
                    }).
                    GetValue(key => new Product
                    {
                        Id = key,
                        Message = $"{key:000}",
                        Timestamp = DateTime.Now,
                    });
                });
            }

            public ICacheStore<int, Product> Products { get; private set; }
        }

        public class Product
        {
            public int Id { get; set; }
            public string Message { get; set; }
            public DateTime Timestamp { get; set; }

            public override string ToString()
            {
                return $"{Id}, {Message}, {Timestamp.TimeOfDay}";
            }
        }
        #endregion
    }
}
