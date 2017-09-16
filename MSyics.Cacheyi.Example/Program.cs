using System;
using MSyics.Cacheyi;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace MSyics.Cacheyi.Example
{
    class Program
    {
        public static MyCacheCenter Cache => new MyCacheCenter();

        static void Main(string[] args)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "test.txt");
            File.WriteAllLines(path, Enumerable.Range(1, 100).Select(i => i.ToString()));

            Example1();

            //var tasks = Enumerable.Range(1, 100).Select(i => Task.Factory.StartNew(() =>
            //{
            //    var value = Cache.MyData.Get(1);
            //    Console.WriteLine(value);
            //}));
            //Task.WaitAll(tasks.ToArray());

            
        }

        private static void Example1()
        {
            {
                // Cache Value
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData.Alloc(1);
                Console.WriteLine(hogeCache.Get());
            }
            System.Threading.Thread.Sleep(1000);
            {
                // Get Cache
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData.Alloc(1);
                Console.WriteLine(hogeCache.Get());
            }
            System.Threading.Thread.Sleep(2100);
            {
                // After Timeout
                var cacheCenter = new MyCacheCenter();
                var hogeCache = cacheCenter.MyData.Alloc(1);
                Console.WriteLine(hogeCache.Get());
            }
        }
    }

    public class MyCacheCenter : CacheCenter
    {
        public CacheStore<int, MyData> MyData { get; set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => MyData).Settings(s =>
                    {
                        
                        s.MaxCapacity = 0;
                        s.Timeout = TimeSpan.FromSeconds(2);
                    })
                    .MakeValue(key =>
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
