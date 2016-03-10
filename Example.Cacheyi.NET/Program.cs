using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example.Cacheyi.NET
{
    using MSyics.Cacheyi;
    using MSyics.Cacheyi.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            {
                var cacheCenter = new HogeCacheCenter();
                Console.WriteLine(cacheCenter.Hoges.MaxCapacity);
            }
            {
                // Cache Value
                var cacheCenter = new HogeCacheCenter();
                var hogeCache = cacheCenter.Hoges.Alloc(Tuple.Create("hogehoge"));
                Console.WriteLine(hogeCache.Get());
                // Get Value : 1
            }
            System.Threading.Thread.Sleep(1000);
            {
                // Get Cache
                var cacheCenter = new HogeCacheCenter();
                var hogeCache = cacheCenter.Hoges.Alloc(Tuple.Create("hogehoge"));
                Console.WriteLine(hogeCache.Get());
                // Get Value : 1
            }
            System.Threading.Thread.Sleep(2100);
            {
                // After Timeout
                var cacheCenter = new HogeCacheCenter();
                var hogeCache = cacheCenter.Hoges.Alloc(Tuple.Create("hogehoge"));
                Console.WriteLine(hogeCache.Get());
                // Get Value : 2
            }
        }
    }

    /// <summary>
    /// Cache Center
    /// </summary>
    public class HogeCacheCenter : CacheCenter
    {
        /// <summary>
        /// Cache Store
        /// </summary>
        public CacheStore<Tuple<string>, int> Hoges { get; set; }

        /// <summary>
        /// Fluent API
        /// </summary>
        /// <param name="director">Build CacheStore</param>
        protected override void ConstructStore(CacheStoreDirector director)
        {
            var i = 0;
            director.Build(() => this.Hoges)
                    .Settings(x =>
                    {
                        x.Read();
                        //x.Timeout = TimeSpan.FromSeconds(3);
                        //x.MaxCapacity = 0;
                    })
                    .MakeUniqueKey(x => x.GetHashCode())
                    .MakeValue(x =>
                    {
                        return ++i;
                    });
        }
    }
}
