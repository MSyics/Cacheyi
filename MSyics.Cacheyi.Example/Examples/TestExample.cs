using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class TestExample : Example
    {
        public override string Name => nameof(TestExample);

        public override void ShowCore()
        {
        }

        public override async Task ShowAsync()
        {
            using (Tracer.Scope())
            {
                //await Add(1, 100);
                for (int i = 0; i < 5; i++)
                {
                    _ = Add(int.MaxValue, 100);
                }
                for (int i = 0; i < 1000; i++)
                {
                    await Task.Delay(10);
                    Read();
                }
            }
        }

        private Task Add(int repeat, int count)
        {
            var c = new HogeHoge().HogeStore;
            return Task.Run(() =>
            {
                for (int i = 0; i < repeat; i++)
                {
                    c.Alloc(Enumerable.Range(0, count));
                    c.Clear();
                    //foreach (var item in c.Alloc(Enumerable.Range(0, 100)))
                    //{
                    //    item.Reset();
                    //}
                }
                Tracer.Information($"end Add");
            });
        }

        private void Read()
        {
            var c = new HogeHoge();
            foreach (var item in c.HogeStore.AsEnumerable())
            {
                var v = item.GetValue();
                Tracer.Information($"{item.Key} {v}");
            }
            Tracer.Information($"end Read");
        }

        public class HogeHoge
        {
            public HogeHoge()
            {
                CacheCenter.ConstructStore(this, director =>
                {
                    director.
                    Build(() => HogeStore).
                    GetValue(key =>
                    {
                        return new Hoge
                        {
                            Id = key,
                            Message = $"{DateTime.Now:ffff}",
                        };
                    });
                });
            }

            public ICacheStore<int, Hoge> HogeStore { get; private set; }
        }

        public class Hoge
        {
            public int Id { get; set; }
            public string Message { get; set; }
        }
    }
}
