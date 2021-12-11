using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class UsingCapacity : Example
    {
        public override string Name => nameof(UsingCapacity);

        public override Task ShowAsync()
        {
            using (Tracer.Scope())
            {
                var center = new ProductCenter();

                foreach (var value in center.Products.Allocate(Enumerable.Range(1, 5)).Select(x => x.GetValue()))
                {
                    Tracer.Information($"{value}");
                }
                foreach (var cache in center.Products.AsEnumerable())
                {
                    Tracer.Information($"{{ {cache.Key}, {cache.Status} }}");
                }

                foreach (var key in Enumerable.Range(6, 3))
                {
                    Tracer.Information($"{center.Products.Allocate(key).GetValue()}");

                    foreach (var cache in center.Products.AsEnumerable())
                    {
                        Tracer.Information($"{{ {cache.Key}, {cache.Status} }}");
                    }
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
                    director.Build(() => Products).
                    Settings(settings =>
                    {
                        settings.MaxCapacity = 5;
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
