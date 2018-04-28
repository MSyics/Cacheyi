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
                var cache = Hoge.Hoge[(1, 2)];
                Console.WriteLine(cache.GetValue());
            }
            {
                var cache = Hoge.Hoge.Alloc((1, 2));
                Console.WriteLine(cache.GetValue());
            }
            {
                var cache = Hoge.Hoge.Alloc((1, 2));
                Console.WriteLine(cache.GetValue());
            }
            {
                var cache = Hoge.Piyo.Alloc(new Data { a = 1, b = 2 });
                Console.WriteLine(cache.GetValue());
                Console.WriteLine(cache.GetValue());
            }
            {
                var cache = Hoge.Piyo.Alloc(new Data { a = 1, b = 2 });
                Console.WriteLine(cache.GetValue());
                Console.WriteLine(cache.GetValue());
            }
            {
                var cache = Hoge.Fuga.Alloc(new Data { a = 1, b = 2 });
                Console.WriteLine(cache.GetValue());
                Console.WriteLine(cache.GetValue());
            }
            {
                var cache = Hoge.Fuga.Alloc(new Data { a = 1, b = 2 });
                Console.WriteLine(cache.GetValue());
                Console.WriteLine(cache.GetValue());
                //cache.Key.KeyedObj.a = 2;
                Console.WriteLine(cache.GetValue());
                //cache.Reset().Get();

                // Hoge.Hoge["aaa"]
                //Hoge.Fuga.Alloc(new Data()).Get().;
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
        public CacheStore<Data, (int a, int b), string> Fuga { get; set; }
        //public CacheStore<string, List<string>> Settings { get; set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => Hoge)
                    .Settings(x =>
                    {
                        x.Timeout = new TimeSpan(1);
                    })
                    .GetValue(key =>
                    {
                        Task.Delay(1000).Wait();
                        return $"{key.a}_{key.a}_{DateTime.Now}";
                    });

            director.Build(() => Piyo)
                    .GetValue(key =>
                    {
                        Task.Delay(1000).Wait();
                        return $"{key.a}{key.b}";
                    });

            director.Build(() => Fuga)
                    .GetKey(x => (x.a, x.b))
                    .GetValue(key =>
                    {
                        Task.Delay(1000).Wait();
                        return $"{key.a}{key.b}";
                    });
        }
    }


}
