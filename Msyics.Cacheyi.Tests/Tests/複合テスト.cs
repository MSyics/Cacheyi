using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;
using Xunit.Abstractions;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class 複合テスト
{
    readonly ITestOutputHelper testOutput;
    readonly ObservableCollection<TestValue> dataSource = new();
    IAsyncCacheStore<int, TestValue> store;

    class Stores : CacheCenter
    {
        public ICacheStore<int, int> StoreA { get; set; }
        public ICacheStore<int, int, int> StoreB { get; set; }
        public IAsyncCacheStore<int, int> StoreC { get; set; }
        public IAsyncCacheStore<int, int, int> StoreD { get; set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => StoreA).Settings(_ => { }).WithMonitoring(null).GetValue((_, __) => _);
            director.Build(() => StoreB).Settings(_ => { }).WithMonitoring(null).GetKey(_ => _).GetValue((_, __) => _);
            director.Build(() => StoreC).Settings(_ => { }).WithMonitoring(null).GetValue((_, __) => Task.FromResult(0));
            director.Build(() => StoreD).Settings(_ => { }).WithMonitoring(null).GetKey(_ => _).GetValue((_, __) => Task.FromResult(0));
        }
    }

    public 複合テスト(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());

    }

    [Fact]
    public void When_ストア構築_Expect_等値()
    {
        Stores stores1 = new();
        Stores stores2 = new();

        Assert.Equal(stores1.StoreA, stores2.StoreA);
        Assert.Equal(stores1.StoreB, stores2.StoreB);
        Assert.Equal(stores1.StoreC, stores2.StoreC);
        Assert.Equal(stores1.StoreD, stores2.StoreD);
    }

    [Fact]
    public async Task When_クリアリセット_Expect_非同期()
    {
        store = new AsyncCacheStore<int, TestValue>(
            valueBuilder: (key, _) => Task.Run(() => dataSource.First(x => x.Key == key)),
            monitoring: new ObservableCollectionMonitoring(),
            maxCapacity: 1,
            timeout: TimeSpan.FromMilliseconds(1),
            timeoutBehaivor: CacheTimeoutBehaivor.Reset);

        await Task.WhenAll(
            Task.WhenAll(Enumerable.Range(1, 1000).Select(_ => Task.Run(store.Clear))),
            Task.WhenAll(Enumerable.Range(1, 1000).Select(_ => store.ResetAsync())),
            Task.WhenAll(Enumerable.Range(1, 1000).Select(_ => Task.Run(() => store.TransferAsync(0, new TestValue(1))))),
            Task.WhenAll(Enumerable.Range(1, 1000).Select(_ => store.Allocate(0).ResetAsync())),
            Task.WhenAll(Enumerable.Range(1, 1000).Select(_ => store.Allocate(0).GetValueAsync().AsTask())));

        var actual = await store.Allocate(0).GetValueAsync();

        Assert.Equal(0, actual.Value);
    }
}
