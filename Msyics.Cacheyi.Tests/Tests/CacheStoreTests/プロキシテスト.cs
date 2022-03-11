using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class プロキシテスト
{
    readonly ObservableCollection<TestValue> dataSource = new();
    readonly ICacheStore<int, TestValue> store;

    public プロキシテスト()
    {
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());
        store = new CacheStore<int, TestValue>((key, _) => dataSource.First(x => x.Key == key));
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
    public void When_取得試行_Expect_状態(int key, CacheStatus status)
    {
        var cache = store.Allocate(key);

        cache.TryGetValue(out var _);

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
    public void When_取得試行_Expect_成否(int key, bool expected)
    {
        var cache = store.Allocate(key);

        var actual = cache.TryGetValue(out var _);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(int.MaxValue, null)]
    public void When_取得試行_Expect_値(int key, int? expected)
    {
        var cache = store.Allocate(key);

        cache.TryGetValue(out var actual);

        Assert.Equal(expected, actual?.Key);
    }

    [Theory]
    [InlineData(0, CacheStatus.Real)]
    public void When_取得済_Expect_状態(int key, CacheStatus status)
    {
        store.Allocate(key).GetValue();

        var cache = store.Allocate(key);

        Assert.Equal(status, cache.Status);
    }

    [Theory]
    [InlineData(0, 0)]
    public void When_取得済_Expect_値(int key, int? expected)
    {
        store.Allocate(key).GetValue();
        var cache = store.Allocate(key);

        var actual = cache.GetValue();

        Assert.Equal(expected, actual?.Key);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    public void When_取得_Expect_失敗(int key)
    {
        var cache = store.Allocate(key);

        Assert.Throws<InvalidOperationException>(() => cache.GetValue());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(int.MaxValue)]
    public void When_ストア解除_Expect_ストアに存在しない(int key)
    {
        var cache = store.Allocate(key);
        cache.TryGetValue(out var _);

        store.Release(key);

        Assert.False(cache.InStock);
    }

    [Fact]
    public void When_リセット_Expect_状態()
    {
        var cache = store.Allocate(0);
        cache.GetValue();
        
        cache.Reset();

        Assert.Equal(CacheStatus.Virtual, cache.Status);
    }

    [Fact]
    public async Task When_タイムアウト時リセット_Expect_タイムアウト()
    {
        store.Timeout = TimeSpan.FromMilliseconds(1);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.None;
        var cache = store.Allocate(0);
        cache.GetValue();

        await Task.Delay(50);
        cache.ResetIfTimeout();

        Assert.Equal(CacheStatus.Virtual, cache.Status);
    }

    [Fact]
    public void When_タイムアウト時リセット_Expect_タイムアウト前()
    {
        store.Timeout = TimeSpan.FromSeconds(1000);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.None;
        var cache = store.Allocate(0);
        cache.GetValue();

        cache.ResetIfTimeout();

        Assert.Equal(CacheStatus.Real, cache.Status);
    }

    [Fact]
    public void When_ストア操作_取得_Expect_状態()
    {
        var cache = store.Allocate(0);

        store.Allocate(0).GetValue();

        Assert.Equal(CacheStatus.Real, cache.Status);
    }

    [Fact]
    public void When_ストア操作_リセット_Expect_状態()
    {
        var cache = store.Allocate(0);
        cache.GetValue();

        store.Allocate(0).Reset();

        Assert.Equal(CacheStatus.Virtual, cache.Status);
    }

    [Theory]
    [InlineData(0, CacheStatus.Real)]
    [InlineData(int.MaxValue, CacheStatus.Virtual)]
    public void When_ストア操作_解除_Expect_状態(int key, CacheStatus expected)
    {
        var cache = store.Allocate(key);
        cache.TryGetValue(out var _);

        store.Release(key);

        Assert.Equal(expected, cache.Status);
    }
}
