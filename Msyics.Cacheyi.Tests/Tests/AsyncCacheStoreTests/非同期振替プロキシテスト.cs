﻿using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class 非同期振替プロキシテスト
{
    readonly ObservableCollection<TestValue> dataSource = new();
    readonly IAsyncCacheStore<int, TestValue> store;

    public 非同期振替プロキシテスト()
    {
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());
        store = new AsyncCacheStore<int, TestValue>((key, _) => Task.Run(() => dataSource.First(x => x.Key == key)));
    }

    [Fact]
    public async Task When_振替_Expect_取得()
    {
        var cache = store.Transfer(int.MaxValue, () => Task.FromResult(new TestValue(0, 0)));

        var actual = (await cache.GetValueAsync()).Value;

        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task When_再振替_Expect_取得()
    {
        var cache = store.Transfer(0, () => Task.FromResult(new TestValue(0, int.MinValue)));
        store.Transfer(0, new TestValue(int.MaxValue, int.MaxValue));

        var actual = (await cache.GetValueAsync()).Value;

        Assert.Equal(int.MaxValue, actual);
    }

    [Fact]
    public async Task When_引当取得済_Expect_状態()
    {
        var cache = store.Allocate(0);
        await cache.GetValueAsync();
        store.Transfer(0, () => Task.FromResult(new TestValue(0, int.MaxValue)));

        Assert.Equal(CacheStatus.Virtual, cache.Status);
    }

    [Fact]
    public async Task When_引当取得済_Expect_取得()
    {
        var cache = store.Allocate(0);
        await cache.GetValueAsync();
        store.Transfer(0, () => Task.FromResult(new TestValue(0, int.MaxValue)));

        var actual = (await cache.GetValueAsync()).Value;

        Assert.Equal(int.MaxValue, actual);
    }
}
