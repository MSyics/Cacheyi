using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class UsingTimeoutBehavior : Example
    {
        public override string Name => nameof(UsingTimeout);

        public override async Task ShowAsync()
        {
            await FireAsync();
        }

        private async Task FireAsync()
        {
            var cache = new ProductCenter();

            using (Tracer.Scope(label: "none"))
            {
                await FireAsync(cache.ProductsForNone);
            }

            using (Tracer.Scope(label: "remove"))
            {
                await FireAsync(cache.ProductsForRemove);
            }

            using (Tracer.Scope(label: "reset"))
            {
                await FireAsync(cache.ProductsForReset);
            }
        }

        private async Task FireAsync(ICacheStore<int, Product> store)
        {
            var cache = new ProductCenter();
            var items = Enumerable.Range(1, 100000).Select(item => store.Allocate(item)).ToArray();

            Tracer.Debug("get");
            foreach (var item in items) item.GetValue();
            for (int i = 1; i < 5; i++)
            {
                Display(store);
                await Task.Delay(1000);
            }

            Tracer.Debug("get");
            foreach (var item in items) item.GetValue();
            Display(store);

            Tracer.Debug("reset if timeout");
            foreach (var item in items)
            {
                item.ResetIfTimeout().GetValue();
            }
            Display(store);

            void Display(ICacheStore<int, Product> store)
            {
                var timedOutCount = store.AsEnumerable().Count(x => x.TimedOut);
                var realCount = store.AsEnumerable().Count(x => x.Status == CacheStatus.Real);
                Tracer.Information($"timeout:{timedOutCount}, real:{realCount}, total:{store.Count}");
            }
        }

        #region Product
        public class ProductCenter
        {
            static readonly TimeSpan timeout = TimeSpan.FromSeconds(2);

            public ProductCenter()
            {
                CacheCenter.ConstructStore(this, director =>
                {
                    director.
                    Build(() => ProductsForNone).
                    Settings(settings =>
                    {
                        settings.Timeout = timeout;
                        settings.TimeoutBehavior = CacheValueTimeoutBehaivor.None;
                    }).
                    GetValue(key => new Product
                    {
                        Id = key,
                        Message = $"{key:000}",
                        Timestamp = DateTime.Now,
                    });

                    director.
                    Build(() => ProductsForRemove).
                    Settings(settings =>
                    {
                        settings.Timeout = timeout;
                        settings.TimeoutBehavior = CacheValueTimeoutBehaivor.Remove;
                    }).
                    GetValue(key => new Product
                    {
                        Id = key,
                        Message = $"{key:000}",
                        Timestamp = DateTime.Now,
                    });

                    director.
                    Build(() => ProductsForReset).
                    Settings(settings =>
                    {
                        settings.Timeout = timeout;
                        settings.TimeoutBehavior = CacheValueTimeoutBehaivor.Reset;
                    }).
                    GetValue(key => new Product
                    {
                        Id = key,
                        Message = $"{key:000}",
                        Timestamp = DateTime.Now,
                    });
                });
            }

            public ICacheStore<int, Product> ProductsForNone { get; private set; }
            public ICacheStore<int, Product> ProductsForRemove { get; private set; }
            public ICacheStore<int, Product> ProductsForReset { get; private set; }
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
