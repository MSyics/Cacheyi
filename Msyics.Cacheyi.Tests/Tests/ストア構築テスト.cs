using MSyics.Cacheyi;
using MSyics.Cacheyi.Monitoring;
using Xunit;
using Xunit.Abstractions;

namespace Msyics.Cacheyi.Tests;

[TestCaseOrderer("Msyics.Cacheyi.Tests.PriorityOrderer", "Msyics.Cacheyi.Tests")]
public partial class �X�g�A�\�z�e�X�g
{
    readonly ITestOutputHelper testOutput;

    public �X�g�A�\�z�e�X�g(ITestOutputHelper testOutput)
    {
        this.testOutput = testOutput;
    }

    [Fact]
    public void When_�Z���^�[�p��_Expect_���l()
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
        public ICacheStore<int, int> Store1 { get; private set; }
        public ICacheStore<int, int> Store2 { get; private set; }

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
                GetValue(x => x);

            director.Build(() => Store2).GetValue(x => x);
        }
    }

    [Fact]
    public void When_�Z���^�[�g�p_Expect_���l()
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
        public ICacheStore<int, int> Store1 { get; private set; }
        public ICacheStore<int, int> Store2 { get; private set; }

        public StoresCenter()
        {
            CacheCenter.ConstructStore(this, director =>
            {
                director.Build(() => Store1).GetValue(x => x);
                director.Build(() => Store2).GetValue(x => x);
            });
        }
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    public void When_�ő�e��_Expect_�ݒ�(int? maxCapacity, bool expected)
    {
        var store = new CacheStore<int, int>(_ => _, maxCapacity: maxCapacity);

        testOutput.WriteLine($"MaxCapacity:{store.MaxCapacity}");
        Assert.Equal(expected, store.HasMaxCapacity);
    }

    [Theory]
    [MemberData(nameof(When_�^�C���A�E�g_Expect_�ݒ�_Case))]
    public void When_�^�C���A�E�g_Expect_�ݒ�(TimeSpan? timeout, bool expected)
    {
        var store = new CacheStore<int, int>(_ => _, timeout: timeout);

        testOutput.WriteLine($"Timeout:{store.Timeout}");
        Assert.Equal(expected, store.HasTimeout);
    }

    static IEnumerable<object?[]> When_�^�C���A�E�g_Expect_�ݒ�_Case()
    {
        yield return new object?[] { null, false };
        yield return new object?[] { TimeSpan.Zero, false };
        yield return new object?[] { TimeSpan.FromMilliseconds(-1), false };
        yield return new object?[] { TimeSpan.FromMilliseconds(1), true };
    }

    [Theory]
    [MemberData(nameof(When_���j�^�����O_Expect_�ݒ�_Case))]
    public void When_���j�^�����O_Expect_�ݒ�(IDataSourceMonitoring<int>? monitoring, bool expected)
    {
        var store = new CacheStore<int, int>(_ => _, monitoring: monitoring);

        Assert.Equal(expected, store.CanMonitoring);
        Assert.Equal(monitoring, store.Monitoring);
    }

    static IEnumerable<object?[]> When_���j�^�����O_Expect_�ݒ�_Case()
    {
        yield return new object?[] { new ObservableCollectionMonitoring(), true };
        yield return new object?[] { null, false };
    }
}
