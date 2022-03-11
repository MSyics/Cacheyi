using MSyics.Traceyi;

namespace MSyics.Cacheyi.Examples;

class SetupForInjection : Example
{
    public override string Name => nameof(SetupForInjection);

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

        Tracer.Information(x => x.value = cache.Products.Allocate(1).GetValue());

        Tracer.Debug("wait");
        await Task.Delay(100);
        Tracer.Information(x => x.value = cache.Products.Allocate(2).GetValue());

        Tracer.Debug("wait");
        await Task.Delay(100);
        Tracer.Information(x => x.value = cache.Products.Allocate(1).GetValue());
    }

    #region Product
    public class ProductCenter
    {
        public ProductCenter()
        {
            Products = new CacheStore<int, Product>(
                valueBuilder: (key, _) => new Product
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
