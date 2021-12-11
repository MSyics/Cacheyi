using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class 振替プロキシテスト
{
    readonly ObservableCollection<TestValue> dataSource = new();
    readonly ICacheStore<int, TestValue> store;

    public 振替プロキシテスト()
    {
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());
        store = new CacheStore<int, TestValue>(key => dataSource.First(x => x.Key == key));
    }

    [Fact]
    public void When_振替_Expect_取得()
    {
        var cache = store.Transfer(int.MaxValue, () => new(0, 0));

        var actual = cache.GetValue().Value;

        Assert.Equal(0, actual);
    }

    [Fact]
    public void When_再振替_Expect_取得()
    {
        var cache = store.Transfer(0, () => new(0, int.MinValue));
        store.Transfer(0, new TestValue(int.MaxValue, int.MaxValue));

        var actual = cache.GetValue().Value;

        Assert.Equal(int.MaxValue, actual);
    }

    [Fact]
    public void When_引当取得済_Expect_状態()
    {
        var cache = store.Allocate(0);
        cache.GetValue();
        store.Transfer(0, () => new(0, int.MaxValue));

        Assert.Equal(CacheStatus.Virtual, cache.Status);
    }

    [Fact]
    public void When_引当取得済_Expect_取得()
    {
        var cache = store.Allocate(0);
        cache.GetValue();
        store.Transfer(0, () => new(0, int.MaxValue));

        var actual = cache.GetValue().Value;

        Assert.Equal(int.MaxValue, actual);
    }
}
