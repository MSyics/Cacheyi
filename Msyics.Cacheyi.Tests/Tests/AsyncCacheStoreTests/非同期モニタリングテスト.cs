using MSyics.Cacheyi;
using System.Collections.ObjectModel;
using Xunit;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class �񓯊����j�^�����O�e�X�g : IDisposable
{
    readonly ObservableCollection<TestValue> dataSource = new();
    readonly IAsyncCacheStore<int, TestValue> store;

    public �񓯊����j�^�����O�e�X�g()
    {
        dataSource.AddRange(Enumerable.Range(0, 5).ToTestValue());
        store = new AsyncCacheStore<int, TestValue>(
            (key, _) => Task.Run(() => dataSource.First(x => x.Key == key)),
            new ObservableCollectionMonitoring { Source = dataSource });
        store.Monitoring.Start();
    }

    public void Dispose() => store.Monitoring.Stop();

    [Fact]
    public async Task When_�������Ȃ�_Expect_�X�g�A�ێ���()
    {
        await store.Allocate(0).GetValueAsync();
        dataSource.Add(new(0));

        Assert.Equal(1, store.Count);
    }

    [Theory]
    [InlineData(new[] { 0, 1 })]
    public async Task When_����_Expect_�X�g�A�ێ���(int[] removeKeys)
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
    public async Task When_�ύX�ӏ����Z�b�g_Expect_�X�g�A�ێ���(int[] modifyKeys, int expected)
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
    public async Task When_���Z�b�g_Expect_�X�g�A�ێ���()
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
    public void When_���ׂĉ���_Expect_�X�g�A�ێ���()
    {
        store.Allocate(dataSource.Select(x => x.Key));

        dataSource.Clear();

        Assert.Equal(0, store.Count);
    }
}
