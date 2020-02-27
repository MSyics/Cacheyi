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
            _ = Add();
            for (int i = 0; i < 10000; i++)
            {
                await Read();
            }
        }

        private Task Add()
        {
            var c = new HogeHoge().HogeStore;
            return Task.Run(() =>
            {
                for (int i = 0; i < int.MaxValue; i++)
                {
                    c.Alloc(Enumerable.Range(0, 100));
                    c.Clear();
                    //foreach (var item in c.Alloc(Enumerable.Range(0, 3000)))
                    //{
                    //    item.Reset();
                    //}
                }
                Tracer.Information($"end Add");
            });
        }

        private Task Read()
        {
            return Task.Run(() =>
            {
                var c = new HogeHoge();
                Tracer.Information($"{c.HogeStore.Count}");
                foreach (var item in c.HogeStore.AsEnumerable().Take(1000))
                {
                    item.GetValue();
                    //Tracer.Information($"{item.Key} {item.GetValue().Message}");
                }
                Tracer.Information($"end Read");
            });
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
