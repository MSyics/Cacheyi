using MSyics.Traceyi;
using System;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class Program : ExampleAggregator
    {
        static void Main(string[] args)
        {
            new Program().
                Add<SetupExample>().
                //Add<TimeoutExample>().
                //Add<ManualAllocExample>().
                //Add<DoOutExample>().
                //Add<DataSourceMonitoringExample>().
                //Add<CacheStorePropertyExample>().
                Add<TestExample>().

                Show();
        }
    }
}
