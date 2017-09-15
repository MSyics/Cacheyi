# Cacheyi

Cacheyi is a micro cache management library, by fluent API.

## Usage

Configuration settings of cache context.
```csharp
public class HogeCacheCenter : CacheCenter
{
    public CacheStore<Tuple<string>, int> Hoges { get; set; }

    protected override void ConstructStore(CacheStoreDirector director)
    {
        var i = 0;
        director.Build(() => this.Hoges)
                .Settings(x =>
                {
                    x.Timeout = TimeSpan.FromSeconds(3);
                    x.MaxCapacity = 10;
                })
                .MakeValue(x =>
                {
                    return ++i;
                });
    }
}
```

Get Cache
```csharp
var cacheCenter = new HogeCacheCenter();
var cache = cacheCenter.Hoges.Alloc(Tuple.Create("hogehoge"));
var value = hogeCache.Get();
```

Example
```csharp
static void Main(string[] args)
{
    {
        var cacheCenter = new HogeCacheCenter();
        var hogeCache = cacheCenter.Hoges.Alloc(Tuple.Create("hogehoge"));
        Console.WriteLine(hogeCache.Get());
        // Get new value : 1
    }
    System.Threading.Thread.Sleep(1000);
    {
        var cacheCenter = new HogeCacheCenter();
        var hogeCache = cacheCenter.Hoges.Alloc(Tuple.Create("hogehoge"));
        Console.WriteLine(hogeCache.Get());
        // Get cache value : 1
    }
    System.Threading.Thread.Sleep(2100);
    {
        var cacheCenter = new HogeCacheCenter();
        var hogeCache = cacheCenter.Hoges.Alloc(Tuple.Create("hogehoge"));
        Console.WriteLine(hogeCache.Get());
        // Get new value after timeout : 2
    }
}
```
