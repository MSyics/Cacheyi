﻿using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class 非同期振替解除テスト
{
    readonly ObservableCollection<TestValue> dataSource = new();
    readonly IAsyncCacheStore<int, TestValue> store;

    public 非同期振替解除テスト()
    {
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());
        store = new AsyncCacheStore<int, TestValue>((key, _) => Task.Run(() => dataSource.First(x => x.Key == key)));
    }

    [Fact]
    public async Task When_取得前_Expect_解除前プロキシから振替した値を取得()
    {
        var cache = store.Transfer(0, new TestValue(0, int.MaxValue));
        store.Release(0);

        var actual = (await cache.GetValueAsync()).Value;

        Assert.Equal(int.MaxValue, actual);
    }

    [Fact]
    public async Task When_取得前_Expect_再引当プロキシから振替前の値を取得()
    {
        store.Transfer(0, new TestValue(0, int.MaxValue));
        store.Release(0);

        var actual = (await store.Allocate(0).GetValueAsync()).Value;

        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task When_取得後_Expect_解除前プロキシをリセットして振替した値を取得()
    {
        var cache = store.Transfer(0, new TestValue(0, int.MaxValue));
        await cache.GetValueAsync();
        store.Release(0);

        cache.Reset();
        var actual = (await cache.GetValueAsync()).Value;

        Assert.Equal(int.MaxValue, actual);
    }

    [Fact]
    public async Task When_タイムアウト_Expect_解除前プロキシから振替した値を取得()
    {
        store.Timeout = TimeSpan.FromMilliseconds(1);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.Release;
        var cache = store.Transfer(0, new TestValue(0, int.MaxValue));
        await cache.GetValueAsync();
        await Task.Delay(50);

        var actual = (await cache.GetValueAsync()).Value;

        Assert.Equal(int.MaxValue, actual);
    }

    [Fact]
    public async Task When_タイムアウト_Expect_再引当プロキシから振替前の値を取得()
    {
        store.Timeout = TimeSpan.FromMilliseconds(1);
        store.TimeoutBehaivor = CacheTimeoutBehaivor.Release;
        await store.Transfer(0, new TestValue(0, int.MaxValue)).GetValueAsync();
        await Task.Delay(50);

        var actual = (await store.Allocate(0).GetValueAsync()).Value;

        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task When_最大保持量オーバー_Expect_解除前プロキシから振替した値を取得()
    {
        store.MaxCapacity = 1;
        var cache = store.Transfer(0, new TestValue(0, int.MaxValue));
        store.Allocate(1);

        var actual = (await cache.GetValueAsync()).Value;

        Assert.Equal(int.MaxValue, actual);
    }

    [Fact]
    public async Task When_最大保持量オーバー_Expect_再引当プロキシから振替前の値を取得()
    {
        store.MaxCapacity = 1;
        store.Transfer(0, new TestValue(0, int.MaxValue));
        store.Allocate(1);

        var actual = (await store.Allocate(0).GetValueAsync()).Value;

        Assert.Equal(0, actual);
    }
}
