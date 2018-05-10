﻿using MSyics.Traceyi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MSyics.Cacheyi
{
    class SetupExample : Example
    {
        class ExampleCacheCenter : CacheCenter
        {
            public CacheStore<int, string> DateTimes { get; set; }

            protected override void ConstructStore(CacheStoreDirector director)
            {
                director.Build(() => DateTimes)
                        .GetValue(x => DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fffffff"));
            }
        }

        ExampleCacheCenter Cache { get; set; } = new ExampleCacheCenter();

        public override void Test()
        {
            Tracer.Debug(Cache.DateTimes[0].GetValue());
            Thread.Sleep(100);
            Tracer.Debug(Cache.DateTimes[0].GetValue());

            Cache.DateTimes[0].Reset();
            Tracer.Debug(Cache.DateTimes[0].GetValue());
        }
    }
}
