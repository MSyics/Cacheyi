using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class UsingAllocate : Example
    {
        public override string Name => nameof(UsingAllocate);

        public override Task ShowAsync()
        {
            using (Tracer.Scope(label: "allocate one from a data source"))
            {
                var center = new ProductCenter();
                
                var cache = center.Products.Allocate(1);
                Tracer.Information($"{cache.Key}, {cache.Status}");
                Tracer.Information($"{cache.GetValue()}");
                Tracer.Information($"{cache.Key}, {cache.Status}");
            }

            using (Tracer.Scope(label: "multipule allocate from a data source"))
            {
                var center = new ProductCenter();

                Tracer.Debug("element with a key of 1 have already been retrieved");
                foreach (var cache in center.Products.Allocate(Enumerable.Range(1, 3)))
                {
                    Tracer.Information($"{cache.Key}, {cache.Status}");
                    Tracer.Information($"{cache.GetValue()}");
                    Tracer.Information($"{cache.Key}, {cache.Status}");
                }
            }

            return Task.CompletedTask;
        }

        #region Product
        public class ProductCenter
        {
            public ProductCenter()
            {
                CacheCenter.ConstructStore(this, director =>
                {
                    director.
                    Build(() => Products).
                    Settings(settings =>
                    {
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
