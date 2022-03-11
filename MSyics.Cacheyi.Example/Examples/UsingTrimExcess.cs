using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class UsingTrimExcess : Example
    {
        public override string Name => nameof(UsingTrimExcess);

        public override Task ShowAsync()
        {
            var center = new ProductCenter();

            center.Products.Allocate(Enumerable.Range(1, 10000));
            Tracer.Information($"allocate count : {center.Products.Count}");

            Tracer.Debug($"get value");
            foreach (var item in center.Products.AsEnumerable().Take(3))
            {
                Tracer.Information($"{item.GetValue()}");
            }

            Tracer.Debug($"TrimExcess");
            center.Products.TrimExcess();
            Tracer.Information($"allocate count : {center.Products.Count}");

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
                    }).
                    GetValue((key, _) => new Product
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
