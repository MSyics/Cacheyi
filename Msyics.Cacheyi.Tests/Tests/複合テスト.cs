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
    ICacheStore<int, TestValue> store;
    IAsyncCacheStore<int, TestValue> asyncStore;

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
    public async Task When_ごちゃまぜ_Expect_非同期()
    {
        asyncStore = new AsyncCacheStore<int, TestValue>(
            valueBuilder: (key, _) => Task.Run(() => new TestValue(key, key)),
            monitoring: new ObservableCollectionMonitoring(),
            maxCapacity: 0,
            timeout: TimeSpan.FromMilliseconds(10),
            timeoutBehaivor: CacheTimeoutBehaivor.Reset);

        await Task.WhenAll(
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => asyncStore.Allocate(0).GetValueAsync().AsTask())),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => asyncStore.Allocate(Enumerable.Range(0, 100)).Select(x => x.GetValueAsync().AsTask())).SelectMany(x => x)),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => asyncStore.Allocate(Enumerable.Range(0, 100)).Select(x => x.TryGetValueAsync().AsTask())).SelectMany(x => x)),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => asyncStore.Transfer(0, new TestValue(1))))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => asyncStore.Transfer(0, () => Task.Run(() => new TestValue(1)))))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => asyncStore.Transfer(0, async token => await Task.Run(() => new TestValue(1)))))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(asyncStore.Allocate(0).Reset))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(asyncStore.Reset))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(asyncStore.Clear))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(asyncStore.TrimExcess))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(asyncStore.AsEnumerable))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => asyncStore.Release(0)))));

        var actual = await asyncStore.Allocate(0).GetValueAsync();

        Assert.Equal(0, actual.Value);
    }

    [Fact]
    public async Task When_ごちゃまぜ_Expect_同期()
    {
        store = new CacheStore<int, TestValue>(
            valueBuilder: (key, _) => new TestValue(key, key),
            monitoring: new ObservableCollectionMonitoring(),
            maxCapacity: 0,
            timeout: TimeSpan.FromMilliseconds(10),
            timeoutBehaivor: CacheTimeoutBehaivor.Reset);

        await Task.WhenAll(
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => store.Allocate(0).GetValue()))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => store.Allocate(Enumerable.Range(0, 100)).Select(x => x.GetValue())))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => store.Allocate(Enumerable.Range(0, 100)).Select(x => x.TryGetValue(out var _))))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => store.Transfer(0, new TestValue(1))))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => store.Transfer(0, () => new TestValue(1))))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => store.Transfer(0, token => new TestValue(1))))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(store.Allocate(0).Reset))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(store.Reset))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(store.Clear))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(store.TrimExcess))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(store.AsEnumerable))),
        Task.WhenAll(Enumerable.Range(1, 2000).Select(_ => Task.Run(() => store.Release(0)))));

        var actual = store.Allocate(0).GetValue();

        Assert.Equal(0, actual.Value);
    }
}
