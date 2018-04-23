using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Example
{
    class Class1
    {
        public Tracer Tracer { get; } = Traceable.Get();

        public HogeCacheCenter Hoge { get; set; } = new HogeCacheCenter();

        public void Test()
        {
            {
                var cache = Hoge.Hoge.Alloc((1, 2));
                Tracer.Information(cache.Get());
                Traceable.Get().Information("");
                Console.WriteLine(cache.Get());
                Console.WriteLine(cache.Get());
                //cache.Key.KeyedObj.a = 2;
            }
            {
                var cache = Hoge.Hoge.Alloc((1, 2));
                Console.WriteLine(cache.Get());
                Console.WriteLine(cache.Get());
            }
            {
                var cache = Hoge.Piyo.Alloc(new Data { a = 1, b = 2 });
                Console.WriteLine(cache.Get());
                Console.WriteLine(cache.Get());
            }
            {
                var cache = Hoge.Piyo.Alloc(new Data { a = 1, b = 2 });
                Console.WriteLine(cache.Get());
                Console.WriteLine(cache.Get());
            }
            {
                var cache = Hoge.Fuga.Alloc(new Data { a = 1, b = 2 });
                Console.WriteLine(cache.Get());
                Console.WriteLine(cache.Get());
            }
            {
                var cache = Hoge.Fuga.Alloc(new Data { a = 1, b = 2 });
                Console.WriteLine(cache.Get());
                Console.WriteLine(cache.Get());
                //cache.Key.KeyedObj.a = 2;
                Console.WriteLine(cache.Get());
                //cache.Reset().Get();
            }
        }
    }

    class Data
    {
        public int a { get; set; }
        public int b { get; set; }
    }

    class HogeCacheCenter : CacheCenter
    {
        public CacheStore<(int a, int b), string> Hoge { get; set; }
        public CacheStore<Data, string> Piyo { get; set; }
        public CacheStore<Data, string, string> Fuga { get; set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => Hoge)
                    .MakeValue(key =>
                    {
                        Task.Delay(1000).Wait();
                        return $"{key.a}{key.b}";
                    });

            director.Build(() => Piyo)
                    .MakeValue(key =>
                    {
                        Task.Delay(1000).Wait();
                        return $"{key.a}{key.b}";
                    });

            director.Build(() => Fuga)
                    .MakeKey(x => $"{x.a}_{x.b}")
                    .MakeValue(keyed =>
                    {
                        Task.Delay(1000).Wait();
                        return $"{keyed.a}{keyed.b}";
                    });
        }
    }


}
