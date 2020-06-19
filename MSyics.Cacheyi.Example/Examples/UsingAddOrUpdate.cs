using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class UsingAddOrUpdate : Example
    {
        public override string Name => nameof(UsingAddOrUpdate);

        public override async Task ShowAsync()
        {
            using (Tracer.Scope(null, "case of add"))
            {
                var center = new ProductCenter();
                var product = new Product { Id = 1, Message = "001", Timestamp = DateTime.Now };
                
                Tracer.Debug($"add {product}");
                center.Products.AddOrUpdate(product.Id, product);

                var cache = center.Products.Allocate(1);
                Tracer.Information($"{cache.Key}, {cache.Status}");
                Tracer.Information($"{cache.GetValue()}");
                Tracer.Information($"{cache.Key}, {cache.Status}");
            }

            using (Tracer.Scope(null, "case of update"))
            {
                var center = new ProductCenter();
                var cache = center.Products.Allocate(1);
                Tracer.Information($"{cache.GetValue()}");

                var product = new Product { Id = 1, Message = "111", Timestamp = DateTime.Now };
                Tracer.Debug($"update {product}");
                center.Products.AddOrUpdate(product.Id, product);

                cache = center.Products.Allocate(1);
                Tracer.Information($"{cache.Key}, {cache.Status}");
                Tracer.Information($"{cache.GetValue()}");
                Tracer.Information($"{cache.Key}, {cache.Status}");
            }

            using (Tracer.Scope(null, "case of timeout"))
            {
                var center = new ProductCenter();
                var cache = center.Products.Allocate(1);
                
                Tracer.Information($"{cache.GetValue()}");
                Tracer.Information($"{cache.Key}, {cache.Status}");

                Tracer.Debug("wait for the timeout");
                await Task.Delay(200);
                
                cache = center.Products.Allocate(1);
                Tracer.Information($"{cache.Key}, {cache.Status}");
                Tracer.Information($"{cache.GetValue()}");
                Tracer.Information($"{cache.Key}, {cache.Status}");
            }
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
                        settings.Timeout = TimeSpan.FromMilliseconds(100);
                    }).
                    GetValue(key => null);
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
