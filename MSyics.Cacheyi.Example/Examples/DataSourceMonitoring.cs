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
    class DataSourceMonitoringExample : Example
    {
        private static readonly string FilePath = "hoge.txt";
        public override string Name => nameof(DataSourceMonitoringExample);

        public override void ShowCore()
        {
            var cache = new HogeCacheCenter();
            cache.HogeStore.Monitoring.Start();

            File.WriteAllLines(FilePath, Enumerable.Range(1, 100).Select(i => i.ToString()));
            {
                var proxy = cache.HogeStore.Alloc(50);
                Tracer.Information(proxy.GetValue().Message);
            }

            File.WriteAllLines(FilePath, Enumerable.Range(101, 100).Select(i => i.ToString()));
            {
                var proxy = cache.HogeStore.Alloc(50);
                Tracer.Information(proxy.GetValue().Message);
            }

        }

        public class HogeCacheCenter : CacheCenter
        {
            public CacheStore<int, Hoge> HogeStore { get; private set; }

            protected override void ConstructStore(CacheStoreDirector director)
            {
                director.Build(() => HogeStore)
                        .Settings(settings =>
                        {
                            settings.MaxCapacity = int.MaxValue;
                            //settings.Timeout = TimeSpan.FromMilliseconds(10);
                        })
                        .WithMonitoring(new FileDataSourceMonitoring())
                        .GetValue(key =>
                        {
                            var line = File.ReadAllLines(FilePath).Skip(key - 1).FirstOrDefault();

                            return new Hoge
                            {
                                Id = key,
                                Message = line,
                            };
                        });
            }
        }

        public class Hoge
        {
            public int Id { get; set; }
            public string Message { get; set; }
        }

        public class FileDataSourceMonitoring : IDataSourceMonitoring<int>
        {
            private readonly FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

            public FileDataSourceMonitoring()
            {
                fileSystemWatcher.Path = Directory.GetCurrentDirectory();
                fileSystemWatcher.Filter = FilePath;
                fileSystemWatcher.Changed += (s, e) =>
                {
                    Changed?.Invoke(this, new DataSourceChangedEventArgs<int>
                    {
                        RefreshWith = RefreshCacheWith.Reset,
                    });
                };
            }

            public bool Running { get; private set; }

            public event EventHandler<DataSourceChangedEventArgs<int>> Changed;

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
    }
}
