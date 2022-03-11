using MSyics.Cacheyi;
using MSyics.Cacheyi.Monitoring;
using Xunit;
using Xunit.Abstractions;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class 非同期ストア構築テスト
{
    readonly ITestOutputHelper testOutput;

    public 非同期ストア構築テスト(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
    }

    [Fact]
    public void When_センター継承_Expect_等値()
    {
        StoresCacheCenter x = new();
        StoresCacheCenter y = new();

        Assert.NotNull(x.Store1);
        Assert.Equal(x.Store1, y.Store1);
        Assert.NotNull(x.Store2);
        Assert.Equal(x.Store2, y.Store2);
    }

    class StoresCacheCenter : CacheCenter
    {
        public IAsyncCacheStore<int, int> Store1 { get; private set; }
        public IAsyncCacheStore<(int, int), int, int> Store2 { get; private set; }

        protected override void ConstructStore(CacheStoreDirector director)
        {
            director.Build(() => Store1).
                Settings(settings =>
                {
                    settings.MaxCapacity = int.MinValue;
                    settings.Timeout = TimeSpan.MinValue;
                    settings.TimeoutBehavior = CacheTimeoutBehaivor.None;
                }).
                WithMonitoring(new ObservableCollectionMonitoring()).
                GetValue((x, _) => Task.Run(() => x));

            director.Build(() => Store2).
                Settings(settings =>
                {
                    settings.MaxCapacity = int.MinValue;
                    settings.Timeout = TimeSpan.MinValue;
                    settings.TimeoutBehavior = CacheTimeoutBehaivor.None;
                }).
                WithMonitoring(new ObservableCollectionMonitoring()).
                GetKey(x => x.Item2).
                GetValue((x, _) => Task.Run(() => x.Item2));
        }
    }

    [Fact]
    public void When_センター使用_Expect_等値()
    {
        StoresCenter x = new();
        StoresCenter y = new();

        Assert.NotNull(x.Store1);
        Assert.Equal(x.Store1, y.Store1);
        Assert.NotNull(x.Store2);
        Assert.Equal(x.Store2, y.Store2);
    }

    class StoresCenter
    {
        public IAsyncCacheStore<int, int> Store1 { get; private set; }
        public IAsyncCacheStore<(int, int), int, int> Store2 { get; private set; }

        public StoresCenter()
        {
            CacheCenter.ConstructStore(this, director =>
            {
                director.Build(() => Store1).
                    Settings(settings =>
                    {
                        settings.MaxCapacity = int.MinValue;
                        settings.Timeout = TimeSpan.MinValue;
                        settings.TimeoutBehavior = CacheTimeoutBehaivor.None;
                    }).
                    WithMonitoring(new ObservableCollectionMonitoring()).
                    GetValue((x, _) => Task.Run(() => x));

                director.Build(() => Store2).
                    Settings(settings =>
                    {
                        settings.MaxCapacity = int.MinValue;
                        settings.Timeout = TimeSpan.MinValue;
                        settings.TimeoutBehavior = CacheTimeoutBehaivor.None;
                    }).
                    WithMonitoring(new ObservableCollectionMonitoring()).
                    GetKey(x => x.Item2).
                    GetValue((x, _) => Task.Run(() => x.Item2));
            });
        }
    }

    [Fact]
    public void When_センター未使用_Expect_不等()
    {
        Stores stores1 = new();
        Stores stores2 = new();

        Assert.NotEqual(stores1.Store1, stores2.Store1);
        Assert.NotEqual(stores1.Store2, stores2.Store2);
    }

    class Stores
    {
        public IAsyncCacheStore<int, int> Store1 { get; private set; }
        public IAsyncCacheStore<(int, int), int, int> Store2 { get; private set; }

        public Stores()
        {
            Store1 = new AsyncCacheStore<int, int>(
                valueBuilder: (_, __) => Task.FromResult(0),
                monitoring: new ObservableCollectionMonitoring(),
                maxCapacity: 0,
                timeout: TimeSpan.Zero,
                timeoutBehaivor: CacheTimeoutBehaivor.None);

            Store2 = new AsyncCacheStore<(int, int), int, int>(
                keyBuilder: _ => 0,
                valueBuilder: (_, __) => Task.FromResult(0),
                monitoring: new ObservableCollectionMonitoring(),
                maxCapacity: 0,
                timeout: TimeSpan.Zero,
                timeoutBehaivor: CacheTimeoutBehaivor.None);
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void When_最大容量_Expect_設定(int? maxCapacity, bool expected)
    {
        var store = new AsyncCacheStore<int, int>((_, __) => Task.FromResult(_), maxCapacity: maxCapacity);

        testOutput.WriteLine($"MaxCapacity:{store.MaxCapacity}");
        Assert.Equal(expected, store.HasMaxCapacity);
    }

    [Theory]
    [MemberData(nameof(When_タイムアウト_Expect_設定_Case))]
    public void When_タイムアウト_Expect_設定(TimeSpan? timeout, bool expected)
    {
        var store = new AsyncCacheStore<int, int>((_, __) => Task.FromResult(_), timeout: timeout);

        testOutput.WriteLine($"Timeout:{store.Timeout}");
        Assert.Equal(expected, store.HasTimeout);
    }

    static IEnumerable<object?[]> When_タイムアウト_Expect_設定_Case()
    {
        yield return new object?[] { null, false };
        yield return new object?[] { TimeSpan.Zero, false };
        yield return new object?[] { TimeSpan.FromMilliseconds(-1), false };
        yield return new object?[] { TimeSpan.FromMilliseconds(1), true };
    }

    [Theory]
    [MemberData(nameof(When_モニタリング_Expect_設定_Case))]
    public void When_モニタリング_Expect_設定(IDataSourceMonitoring<int>? monitoring, bool expected)
    {
        var store = new AsyncCacheStore<int, int>((_, __) => Task.FromResult(_), monitoring: monitoring);

        Assert.Equal(expected, store.CanMonitoring);
        Assert.Equal(monitoring, store.Monitoring);
    }

    static IEnumerable<object?[]> When_モニタリング_Expect_設定_Case()
    {
        yield return new object?[] { new ObservableCollectionMonitoring(), true };
        yield return new object?[] { null, false };
    }
}
