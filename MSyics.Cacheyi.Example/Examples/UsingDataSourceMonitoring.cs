using MSyics.Cacheyi.Monitoring;
using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class UsingDataSourceMonitoring : Example
    {
        private static readonly string filePath = "products.txt";
        public override string Name => nameof(UsingDataSourceMonitoring);

        public override async Task ShowAsync()
        {
            CretaeFile();
            var center = new ProductCenter();
            center.Products.Monitoring.Start();

            var cache = center.Products.Allocate(0);
            Tracer.Information($"{cache.Key}, {cache.Status}");
            Tracer.Information($"{cache.GetValue()}");
            Tracer.Information($"{cache.Key}, {cache.Status}");

            Tracer.Debug($"update the file and wait for change notifications");
            CretaeFile();
            await Task.Delay(200);

            cache = center.Products.Allocate(0);
            Tracer.Information($"{cache.Key}, {cache.Status}");
            Tracer.Information($"{cache.GetValue()}");
            Tracer.Information($"{cache.Key}, {cache.Status}");
        }

        private static void CretaeFile()
        {
            File.WriteAllLines(filePath, Enumerable.Range(0, 10).Select(key => $"{key},ProductA,{DateTime.Now:yyyy/MM/dd HH:mm:ss.fffffff}"));
        }

        #region Product
        public class ProductCenter
        {
            public ProductCenter()
            {
                CacheCenter.ConstructStore(this, director =>
                {
                    CacheStoreDirector.Build(() => Products).
                    Settings(settings =>
                    {
                    }).
                    WithMonitoring(new FileDataSourceMonitoring()).
                    GetValue(key =>
                    {
                        var items = File.ReadLines(filePath).Skip(key).FirstOrDefault()?.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        if (items?.Length != 3) return null;
                        if (!int.TryParse(items[0], out var id)) return null;
                        if (!DateTime.TryParse(items[2], out var timestamp)) return null;

                        return new Product
                        {
                            Id = id,
                            Message = items[1],
                            Timestamp = timestamp,
                        };
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

        #region DataSourceMonitoring
        public class FileDataSourceMonitoring : IDataSourceMonitoring<int>
        {
            private readonly FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            public FileDataSourceMonitoring()
            {
                fileSystemWatcher.Path = Directory.GetCurrentDirectory();
                fileSystemWatcher.Filter = filePath;
                fileSystemWatcher.Changed += (s, e) =>
                {
                    DataSourceChanged?.Invoke(this, new DataSourceChangedEventArgs<int>
                    {
                        RefreshWith = RefreshCacheWith.Reset,
                    });
                };
            }

            public bool Running { get; private set; }

            public event EventHandler<DataSourceChangedEventArgs<int>> DataSourceChanged;

            public void Start()
            {
                if (Running) return;

                Running = true;
                fileSystemWatcher.EnableRaisingEvents = true;
            }

            public void Stop()
            {
                if (!Running) return;

                Running = false;
                fileSystemWatcher.EnableRaisingEvents = false;
            }
        }
        #endregion
    }
}
