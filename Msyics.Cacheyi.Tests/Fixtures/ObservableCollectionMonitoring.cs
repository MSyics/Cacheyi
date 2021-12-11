using MSyics.Cacheyi.Monitoring;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Msyics.Cacheyi.Tests;

public class ObservableCollectionMonitoring : IDataSourceMonitoring<int>
{
    public ObservableCollection<TestValue> Source { get; set; }

    public bool Running { get; private set; }

    public event EventHandler<DataSourceChangedEventArgs<int>>? DataSourceChanged;

    public void Start()
    {
        if (Running) { return; }

        Source.CollectionChanged += OnDataSourceCollectionChanged;
        Running = true;
    }

    public void Stop()
    {
        if (!Running) { return; }

        Source.CollectionChanged -= OnDataSourceCollectionChanged;
        Running = false;
    }

    private void OnDataSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                DataSourceChanged?.Invoke(this, new DataSourceChangedEventArgs<int>
                {
                    RefreshWith = RefreshCacheWith.None,
                    Keys = GetValues(e.NewItems),
                });
                break;
            case NotifyCollectionChangedAction.Remove:
                DataSourceChanged?.Invoke(this, new DataSourceChangedEventArgs<int>
                {
                    RefreshWith = RefreshCacheWith.Release,
                    Keys = GetValues(e.OldItems),
                });
                break;
            case NotifyCollectionChangedAction.Replace:
                DataSourceChanged?.Invoke(this, new DataSourceChangedEventArgs<int>
                {
                    RefreshWith = RefreshCacheWith.ResetContains,
                    Keys = GetValues(e.OldItems),
                });
                break;
            case NotifyCollectionChangedAction.Move:
                DataSourceChanged?.Invoke(this, new DataSourceChangedEventArgs<int>
                {
                    RefreshWith = RefreshCacheWith.Reset,
                    Keys = GetValues(e.OldItems),

                });
                break;
            case NotifyCollectionChangedAction.Reset:
            default:
                DataSourceChanged?.Invoke(this, new DataSourceChangedEventArgs<int>
                {
                    RefreshWith = RefreshCacheWith.Clear,
                    Keys = GetValues(e.OldItems),
                });
                break;
        }

        static int[] GetValues(IList? items)
        {
            if (items == null) return Enumerable.Empty<int>().ToArray();
            return items.OfType<TestValue>().Select(x => x.Key).ToArray();
        }

    }
}
