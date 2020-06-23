using MSyics.Traceyi;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{

    class Program : ExampleAggregator
    {
        static Task Main(string[] args)
        {
            return new Program().
                //Add<SetupByCacheCenter>().
                //Add<SetupByCacheStoreProperty>().
                //Add<SetupForInjection>().
                //Add<UsingAllocate>().
                //Add<UsingAddOrUpdate>().
                //Add<UsingCapacity>().
                //Add<UsingTimeout>().
                //Add<UsingAdjust>().
                //Add<UsingDataSourceMonitoring>().
                Add<ExampleOfLoad>().

                ShowAsync();
        }
    }
}
