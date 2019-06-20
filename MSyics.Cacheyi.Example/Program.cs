using MSyics.Traceyi;
using System;
using System.Threading.Tasks;

namespace MSyics.Cacheyi.Examples
{
    class Program : ExampleAggregator
    {
        static async Task Main(string[] args)
        {
            await new Program()
                //.Add<SetupExample>()
                //.Add<TimeoutExample>()
                .Add<AddExample>()

                .ShowAsync();
        }
    }
}
