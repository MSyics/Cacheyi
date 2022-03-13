using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class 非同期プロキシテスト
{
    readonly ObservableCollection<TestValue> dataSource = new();
    readonly IAsyncCacheStore<int, TestValue> store;

    public 非同期プロキシテスト()
    {
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());
        store = new AsyncCacheStore<int, TestValue>((key, _) => Task.Run(() => dataSource.First(x => x.Key == key)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    public void When_取得前_Expect_状態(int key)
    {
        var cache = store.Allocate(key);

        Assert.Equal(CacheStatus.Virtual, cache.Status);
        Assert.Equal(key, cache.Key);
        Assert.Equal(TimeSpan.Zero, cache.Timeout);
        Assert.Equal(CacheTimeoutBehaivor.Reset, cache.TimeoutBehaivor);
        Assert.True(cache.InStock);
        Assert.False(cache.HasTimeout);
        Assert.False(cache.TimedOut);
    }

    [Theory]
    [InlineData(0, CacheStatus.Real)]
    [InlineData(int.MaxValue, CacheStatus.Virtual)]
    public async Task When_取得試行_Expect_状態(int key, CacheStatus status)
    {
        var cache = store.Allocate(key);

        _ = await cache.TryGetValueAsync();

        Assert.Equal(status, cache.Status);
        Assert.Equal(key, cache.Key);
        Assert.Equal(TimeSpan.Zero, cache.Timeout);
        Assert.Equal(CacheTimeoutBehaivor.Reset, cache.TimeoutBehaivor);
        Assert.True(cache.InStock);
        Assert.False(cache.HasTimeout);
        Assert.False(cache.TimedOut);
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(int.MaxValue, false)]
    public async Task When_取得試行_Expect_成否(int key, bool expected)
    {
        var cache = store.Allocate(key);

        var (actual, _) = await cache.TryGetValueAsync();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(int.MaxValue, null)]
    public async Task When_取得試行_Expect_値(int key, int? expected)
    {
        var cache = store.Allocate(key);

        var (_, actual) = await cache.TryGetValueAsync();

        Assert.Equal(expected, actual?.Key);
    }

    [Theory]
    [InlineData(0, CacheStatus.Real)]
    public async Task When_取得済_Expect_状態(int key, CacheStatus status)
    {
        await store.Allocate(key).GetValueAsync();

        var cache = store.Allocate(key);

        Assert.Equal(status, cache.Status);
    }

    [Theory]
    [InlineData(0, 0)]
    public async Task When_取得済_Expect_値(int key, int? expected)
    {
        await store.Allocate(key).GetValueAsync();
        var cache = store.Allocate(key);

        var actual = await cache.GetValueAsync();

        Assert.Equal(expected, actual?.Key);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    public async Task When_取得_Expect_失敗(int key)
    {
        var cache = store.Allocate(key);

        await Assert.ThrowsAsync<InvalidOperationException>(async () => await cache.GetValueAsync());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    public async Task When_ストア解除_Expect_ストアに存在しない(int key)
    {
        var cache = store.Allocate(key);
        await cache.TryGetValueAsync();

        store.Release(key);

        Assert.False(cache.InStock);
    }

    [Fact]
    public async Task When_リセット_Expect_状態()
    {
        var cache = store.Allocate(0);
        await cache.GetValueAsync();

        cache.Reset();

        Assert.Equal(CacheStatus.Virtual, cache.Status);
    }

    [Fact]
    public async Task When_タイムアウト時リセット_Expect_タイムアウト()
    {
        store.Timeout = TimeSpan.FromMilliseconds(1);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.None;
        var cache = store.Allocate(0);
        await cache.GetValueAsync();

        await Task.Delay(50);
        cache.ResetIfTimeout();

        Assert.Equal(CacheStatus.Virtual, cache.Status);
    }

    [Fact]
    public async Task When_タイムアウト時リセット_Expect_タイムアウト前()
    {
        store.Timeout = TimeSpan.FromSeconds(1000);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.None;
        var cache = store.Allocate(0);
        await cache.GetValueAsync();

        cache.ResetIfTimeout();

        Assert.Equal(CacheStatus.Real, cache.Status);
    }

    [Fact]
    public async Task When_ストア操作_取得_Expect_状態()
    {
        var cache = store.Allocate(0);

        await store.Allocate(0).GetValueAsync();

        Assert.Equal(CacheStatus.Real, cache.Status);
    }

    [Fact]
    public async Task When_ストア操作_リセット_Expect_状態()
    {
        var cache = store.Allocate(0);
        await cache.GetValueAsync();

        store.Allocate(0).Reset();

        Assert.Equal(CacheStatus.Virtual, cache.Status);
    }

    [Theory]
    [InlineData(0, CacheStatus.Real)]
    [InlineData(int.MaxValue, CacheStatus.Virtual)]
    public async Task When_ストア操作_解除_Expect_状態(int key, CacheStatus expected)
    {
        var cache = store.Allocate(key);
        await cache.TryGetValueAsync();

        store.Release(key);

        Assert.Equal(expected, cache.Status);
    }
}
