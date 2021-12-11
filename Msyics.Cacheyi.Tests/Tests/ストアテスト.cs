using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class ストアテスト
{
    readonly ObservableCollection<TestValue> dataSource = new();
    readonly ICacheStore<int, TestValue> store;

    public ストアテスト()
    {
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());
        store = new CacheStore<int, TestValue>(key => dataSource.First(x => x.Key == key));
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(int.MaxValue, 1)]
    public void When_引当_Expect_保持数(int key, int expected)
    {
        store.Allocate(key);

        Assert.Equal(expected, store.Count);
    }

    [Theory]
    [InlineData(new[] { 0, 0 }, 1)]
    [InlineData(new[] { 0, int.MaxValue }, 2)]
    public void When_一括引当_Expect_保持数(int[] keys, int expected)
    {
        store.Allocate(keys);

        Assert.Equal(expected, store.Count);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(int.MaxValue, int.MaxValue, 0)]
    public void When_解除_Expect_保持数(int allocateKeys, int removeKeys, int expected)
    {
        store.Allocate(allocateKeys);

        store.Release(removeKeys);

        Assert.Equal(expected, store.Count);
    }

    [Theory]
    [InlineData(new[] { 0, 0 }, new[] { 0, 0 }, 0)]
    [InlineData(new[] { 0, int.MaxValue }, new[] { 0 }, 1)]
    public void When_一括解除_Expect_保持数(int[] allocateKeys, int[] removeKeys, int expected)
    {
        store.Allocate(allocateKeys);

        store.Release(removeKeys);

        Assert.Equal(expected, store.Count);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(new[] { 0, int.MaxValue })]
    public void When_すべて解除_Expect_保持数(int[] allocateKeys)
    {
        store.Allocate(allocateKeys);

        store.Clear();

        Assert.Equal(0, store.Count);
    }

    [Theory]
    [InlineData(new[] { 0 }, 1)]
    [InlineData(new[] { int.MaxValue }, 0)]
    [InlineData(new[] { 0, int.MaxValue }, 1)]
    public void When_整理_Expect_保持数(int[] allocateKeys, int expected)
    {
        foreach (var cache in store.Allocate(allocateKeys))
        {
            cache.TryGetValue(out var _);
        }

        store.TrimExcess();

        Assert.Equal(expected, store.Count);
    }

    [Theory]
    [InlineData(1, new[] { 0, int.MaxValue })]
    public void When_最大保持量_Expect_保持数(int capacity, int[] keys)
    {
        store.MaxCapacity = capacity;
        store.Allocate(keys);

        Assert.Equal(capacity, store.Count);
    }

    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void When_最大保持量_Expect_有無(int capacity, bool expected)
    {
        store.MaxCapacity = capacity;

        Assert.Equal(expected, store.HasMaxCapacity);
    }

    [Theory]
    [InlineData(1, new[] { 0, int.MaxValue })]
    public void When_最大保持量_Expect_先入解除(int capacity, int[] keys)
    {
        store.MaxCapacity = capacity;
        store.Allocate(keys);

        var expected = store.AsEnumerable().Last().Key;
        var actual = keys.Last();

        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(1, new[] { 0, int.MaxValue })]
    public void When_最大保持量_Expect_解除(int capacity, int[] keys)
    {
        store.MaxCapacity = capacity;
        store.Allocate(keys);

        store.MaxCapacity = 0;
        store.Allocate(keys);

        Assert.True(capacity < store.Count);
    }

    [Fact]
    public void When_リセット_Expect_状態()
    {
        foreach (var cache in store.Allocate(dataSource.Select(x => x.Key)))
        {
            cache.GetValue();
        }

        store.Reset();

        foreach (var cache in store.AsEnumerable())
        {
            Assert.Equal(CacheStatus.Virtual, cache.Status);
        }
    }

    [Fact]
    public void When_振替_Expect_保持数()
    {
        store.Transfer(0, new TestValue(0, int.MaxValue));

        Assert.Equal(1, store.Count);
    }

    [Fact]
    public async Task When_タイムアウト_Expect_解除()
    {
        store.Timeout = TimeSpan.FromMilliseconds(1);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.Release;
        store.Allocate(0).GetValue();

        await Task.Delay(50);

        Assert.Equal(0, store.Count);
    }

    [Fact]
    public async Task When_タイムアウト_Expect_リセット()
    {
        store.Timeout = TimeSpan.FromMilliseconds(1);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.Reset;
        store.Allocate(0).GetValue();

        await Task.Delay(50);

        Assert.Equal(CacheStatus.Virtual, store.AsEnumerable().First().Status);
    }

    [Fact]
    public async Task When_タイムアウト_Expect_何もしない()
    {
        store.Timeout = TimeSpan.FromMilliseconds(1);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.None;
        store.Allocate(0).GetValue();

        await Task.Delay(50);

        Assert.True(store.HasTimeout);
        Assert.Equal(CacheStatus.Real, store.AsEnumerable().First().Status);
    }
}
