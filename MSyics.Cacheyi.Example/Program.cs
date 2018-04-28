using System;
using MSyics.Cacheyi;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MSyics.Cacheyi.Example
{
    class Program : Examplar
    {
        public static MyCacheCenter Cache => new MyCacheCenter();

        static void Main(string[] args)
        {
            new Program()
                .Add<SetupCacheStore>()

                .Test();

            return;

            var c = new Class1();
            c.Test();

            return;


            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.WriteAllLines(path, Enumerable.Range(1, 100).Select(i => i.ToString()));

            //Example1();
            //Example2();

            var key = new Example.MyData()
            {
                Id = 1,
                Created = DateTime.Now,
                Message = Guid.NewGuid().ToString(),
            };

            {
                var tasks = Enumerable.Range(1, 10).Select(i => Task.Factory.StartNew(() =>
                {
                    var cache = Cache.MyData2.Alloc(key);
                    var value = cache.GetValue();
                    Console.WriteLine(value);
                }));
                Task.WaitAll(tasks.ToArray());
            }
            {
                var tasks = Enumerable.Range(1, 100).Select(i => Task.Factory.StartNew(() =>
                {
                    var cache = Cache.MyData.Alloc(1);
                    var value = cache.GetValue();
                    Console.WriteLine(value);
                }));
                Task.WaitAll(tasks.ToArray());
            }
        }

        private static void Example1()
        {
            System.Threading.Thread.Sleep(1000);
            {
                // Get Cache
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData.Alloc(1);
                Console.WriteLine(hogeCache.GetValue());
            }
            System.Threading.Thread.Sleep(1000);
            {
                // Get Cache
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData.Alloc(1);
                Console.WriteLine(hogeCache.GetValue());
            }
            System.Threading.Thread.Sleep(2100);
            {
                // After Timeout
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData.Alloc(1);
                Console.WriteLine(hogeCache.GetValue());
            }
        }

        private static void Example2()
        {
            var key = new Example.MyData()
            {
                Id = 1,
                Created = DateTime.Now,
                Message = Guid.NewGuid().ToString(),
            };
            {
                // Cache Value
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData2.Alloc(key);
                Console.WriteLine(hogeCache.GetValue());
            }
            System.Threading.Thread.Sleep(10000);
            {
                // Get Cache
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData2.Alloc(key);
                Console.WriteLine(hogeCache.GetValue());
            }
            System.Threading.Thread.Sleep(2100);
            {
                // After Timeout
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData2.Alloc(key);
                Console.WriteLine(hogeCache.GetValue());
            }
        }
    }

    public class MyCacheCenter : CacheCenter
    {
        public CacheStore<int, MyData> MyData { get; set; }
        public CacheStore<MyData, int, MyData> MyData2 { get; set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => MyData).Settings(s =>
                    {

                        s.MaxCapacity = 0;
                        s.Timeout = TimeSpan.FromMilliseconds(1);
                    })
                    .GetValue(key =>
                    {
                        var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
                        return new Example.MyData()
                        {
                            Id = key,
                            Created = DateTime.Now,
                            Message = Guid.NewGuid().ToString(),
                            Test = File.ReadLines(path).Last(),
                        };
                    });

            director.Build(() => MyData2).Settings(s =>
            {
                s.MaxCapacity = 0;
                s.Timeout = TimeSpan.FromTicks(2);
            })
            .GetKey(x =>
            {
                return x.Id;
            })
            .GetValue(key =>
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
                return new Example.MyData()
                {
                    Id = key,
                    Created = DateTime.Now,
                    Message = Guid.NewGuid().ToString(),
                    Test = "",//File.ReadLines(path).Where(x => x == key.Id.ToString()).Single(),
                };
            });
        }
    }

    public class MyData
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public string Message { get; set; }
        public string Test { get; set; }

        public override string ToString()
        {
            return $"{Id},{Created:yyyyMMddHHmmssffffff},{Message},{Test}";
        }
    }
}
