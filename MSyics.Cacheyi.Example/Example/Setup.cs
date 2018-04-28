using System;
using System.Collections.Generic;
using System.Text;

namespace MSyics.Cacheyi.Example
{
    class SetupCacheStore : IExample
    {
        class Center : CacheCenter
        {
            public CacheStore<int, DateTime> DateTimeStore { get; set; }
            

            protected override void ConstructStore(CacheStoreDirector director)
            {
                director.Build(() => DateTimeStore)
                        .GetValue(x =>
                        {
                            return DateTime.Now;
                        });
            }
        }

        public void Setup()
        {

        }

        Center Hoge { get; set; } = new Center();
        public CacheStore<int, DateTime> DateTimeStore2
        {
            get
            {
                return Cacheable.Setup(() => DateTimeStore2, config =>
                {
                    config.GetValue(x =>
                    {
                        return DateTime.Now;
                    });
                });
            }
        }

        public void Test()
        {
            Console.WriteLine(DateTimeStore2[0].GetValue());
        }
    }
}
