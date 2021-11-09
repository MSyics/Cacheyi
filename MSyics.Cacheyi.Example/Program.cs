using MSyics.Cacheyi.Examples;

class Program : ExampleAggregator
{
    static Task Main()
    {
        return new Program().
            //Add<SetupByCacheCenter>().
            //Add<SetupByCacheStoreProperty>().
            //Add<SetupForInjection>().
            //Add<UsingAllocate>().
            //Add<UsingAddOrUpdate>().
            //Add<UsingCapacity>().
            //Add<UsingTimeout>().
            //Add<UsingTimeoutBehavior>().
            //Add<UsingTrimExcess>().
            //Add<UsingDataSourceMonitoring>().
            Add<ExampleOfLoad>().

            ShowAsync();
    }
}
