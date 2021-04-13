using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class ExampleOfLoad : Example
    {
        public override string Name => nameof(ExampleOfLoad);

        public override async Task ShowAsync()
        {
            for (int i = 0; i < 1000; i++)
            {
                _ = LoadAsync(i);
            }
            await Task.Delay(100);
            await GetAsync();
        }

        private Task LoadAsync(int number)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(50);
                var store = new ProductCenter().Products;
                for (int i = 0; i < 5; i++)
                {
                    store.AddOrUpdate(i, new Product { Id = i, Message = $"{number}_{i:000}", Timestamp = DateTime.Now });
                }
            });
        }

        private Task GetAsync()
        {
            return Task.Run(() =>
            {
                foreach (var cache in new ProductCenter().Products.AsEnumerable())
                {
                    Tracer.Information($"{cache.Key}, {cache.Status}");
                    Tracer.Information($"{cache.GetValue()}");
                    Tracer.Information($"{cache.Key}, {cache.Status}");
                }
            });
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
