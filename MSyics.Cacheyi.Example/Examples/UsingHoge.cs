using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class UsingHoge : Example
    {
        public override string Name => nameof(UsingHoge);

        public override async Task ShowAsync()
        {
            await Task.WhenAll(Enumerable.Range(1, 20).Select(_ => Hoge()).ToArray());
        }

        private async Task Hoge()
        {
            await Task.WhenAll(
                Piyo(),
                Fuga()
                //Foo()
                );
        }

        private Task Piyo()
        {
            return Task.Run(() =>
            {
                var rnd = new Random(DateTime.Now.Millisecond);
                foreach (var key in Enumerable.Range(1, 1000))
                {
                    var value = center.Products.Allocate(rnd.Next(1, 1)).GetValue();
                    Tracer.Information(value);
                }
            });
        }

        private Task Fuga()
        {
            _ = Task.Run(() =>
            {
                foreach (var key in Enumerable.Range(1, 100000000))
                {
                    center.Products.Reset();
                }
            });

            return Task.CompletedTask;
        }

        private Task Foo()
        {
            return Task.Run(() =>
            {
                center.Products.Clear();
            });
        }

        private ProductCenter center = new ProductCenter();

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
                        //settings.Timeout = TimeSpan.FromMilliseconds(100000);
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
