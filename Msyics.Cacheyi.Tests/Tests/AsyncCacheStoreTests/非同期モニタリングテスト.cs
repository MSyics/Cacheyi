using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class 非同期モニタリングテスト : IDisposable
{
    readonly ObservableCollection<TestValue> dataSource = new();
    readonly IAsyncCacheStore<int, TestValue> store;

    public 非同期モニタリングテスト()
    {
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());
        store = new AsyncCacheStore<int, TestValue>(
            (key, _) => Task.Run(() => dataSource.First(x => x.Key == key)),
            new ObservableCollectionMonitoring { Source = dataSource });
        store.Monitoring.Start();
    }

    public void Dispose() => store.Monitoring.Stop();

    [Fact]
    public async Task When_何もしない_Expect_ストア保持数()
    {
        await store.Allocate(0).GetValueAsync();
        dataSource.Add(new(0));

        Assert.Equal(1, store.Count);
    }

    [Theory]
    [InlineData(new[] { 0, 1 })]
    public async Task When_解除_Expect_ストア保持数(int[] removeKeys)
    {
        foreach (var cache in store.Allocate(dataSource.Select(x => x.Key)))
        {
            await cache.GetValueAsync();
        }
        foreach (var key in removeKeys)
        {
            dataSource.RemoveAt(key);
        }

        Assert.Equal(dataSource.Count, store.Count);
    }

    [Theory]
    [InlineData(new[] { 0, 1 }, 2)]
    public async Task When_変更箇所リセット_Expect_ストア保持数(int[] modifyKeys, int expected)
    {
        foreach (var cache in store.Allocate(dataSource.Select(x => x.Key)))
        {
            await cache.GetValueAsync();
        }
        foreach (var key in modifyKeys)
        {
            dataSource[key] = new(key);
        }

        var actual = store.AsEnumerable().Count(x => x.Status == CacheStatus.Virtual);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task When_リセット_Expect_ストア保持数()
    {
        foreach (var cache in store.Allocate(dataSource.Select(x => x.Key)))
        {
            await cache.GetValueAsync();
        }
        dataSource.Move(0, 1);

        var actual = store.AsEnumerable().Count(x => x.Status == CacheStatus.Virtual);

        Assert.Equal(store.Count, actual);
    }

    [Fact]
    public void When_すべて解除_Expect_ストア保持数()
    {
        store.Allocate(dataSource.Select(x => x.Key));

        dataSource.Clear();

        Assert.Equal(0, store.Count);
    }
}
