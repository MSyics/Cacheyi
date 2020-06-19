using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class SetupForInjection : Example
    {
        public override string Name => nameof(SetupForInjection);

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
        public class ProductCenter
        {
            public ProductCenter()
            {
                Products = new CacheStore<int, Product>(
                    valueBuilder: key => new Product
                    {
                        Id = key,
                        Message = $"{key:000}",
                        Timestamp = DateTime.Now,
                    },
                    maxCapacity: 100);
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
