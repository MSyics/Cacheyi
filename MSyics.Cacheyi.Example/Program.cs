using MSyics.Traceyi;
using System;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class Program : ExampleAggregator
    {
        static async Task Main(string[] args)
        {
            var p = new Program();
            p.
                //Add<SetupExample>().
                //Add<TimeoutExample>().
                //Add<ManualAllocExample>().
                //Add<DoOutExample>().
                //Add<DataSourceMonitoringExample>().
                //Add<CacheStorePropertyExample>().
                Add<TestExample>();

            //p.Show();
            await p.ShowAsync();
        }
    }
}
