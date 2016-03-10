/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.IO;
using System.Threading;
using System.Collections.ObjectModel;

namespace MSyics.Cacheyi.Monitors
{
    /// <summary>
    /// ファイルシステムの変更通知を実装するための基本クラスです。
    /// </summary>
    /// <typeparam name="TKey">キーの型</typeparam>
    public abstract class FileChangeMonitor<TKey> : IDataSourceChangeMonitor<TKey>
    {
        private FileSystemWatcher m_watcher = new FileSystemWatcher();

        /// <summary>
        /// FileChangeMonitor のインスタンスを初期化します。
        /// </summary>
        public FileChangeMonitor()
        {
            this.Waiting = true;
            m_watcher.Path = this.Path;
            m_watcher.Filter = this.Filter;
            m_watcher.NotifyFilter = this.NotifyFilter;
        }

        /// <summary>
        /// データソースが変更されると発生します。
        /// </summary>
        public event EventHandler<DataSourceChangeEventArgs<TKey>> OnChanged;

        /// <summary>
        /// データソースの変更通知を開始します。
        /// </summary>
        public void Start()
        {
            if (!this.Waiting) { return; }

            m_watcher.Changed += new FileSystemEventHandler(Handler);
            m_watcher.Deleted += new FileSystemEventHandler(Handler);
            m_watcher.Created += new FileSystemEventHandler(Handler);
            m_watcher.Deleted += new FileSystemEventHandler(Handler);
            m_watcher.Renamed += new RenamedEventHandler(Handler);

            this.Waiting = false;
        }

        /// <summary>
        /// データソースの変更通知を停止します。
        /// </summary>
        public void Stop()
        {
            if (this.Waiting) { return; }

            m_watcher.Changed -= new FileSystemEventHandler(Handler);
            m_watcher.Deleted -= new FileSystemEventHandler(Handler);
            m_watcher.Created -= new FileSystemEventHandler(Handler);
            m_watcher.Deleted -= new FileSystemEventHandler(Handler);
            m_watcher.Renamed -= new RenamedEventHandler(Handler);

            this.Waiting = true;
        }

        private void Handler(object sender, RenamedEventArgs e)
        {
            var receive = Interlocked.CompareExchange(ref OnChanged, null, null);
            if (receive != null)
            {
                var dsce = new DataSourceChangeEventArgs<TKey>()
                {
                    ChangeAction = CacheChangeAction.None,
                    Keys = new Collection<TKey>(),
                };
                CreateEventArgs(e, dsce);
                receive(this, dsce);
            }
        }

        private void Handler(object sender, FileSystemEventArgs e)
        {
            var receive = Interlocked.CompareExchange(ref OnChanged, null, null);
            if (receive != null)
            {
                var dsce = new DataSourceChangeEventArgs<TKey>()
                {
                    ChangeAction = CacheChangeAction.None,
                    Keys = new Collection<TKey>(),
                };
                CreateEventArgs(e, dsce);
                receive(this, dsce);
            }
        }

        /// <summary>
        /// データソース変更時のイベントデータを作成します。
        /// </summary>
        /// <param name="fse">通知イベントの一連の引数</param>
        /// <param name="dsce">データソース変更時の一連のデータ</param>
        protected abstract void CreateEventArgs(FileSystemEventArgs fse, DataSourceChangeEventArgs<TKey> dsce);

        /// <summary>
        /// データソース変更時のイベントデータを作成します。
        /// </summary>
        /// <param name="rse">通知イベントの一連の引数</param>
        /// <param name="dsce">データソース変更時の一連のデータ</param>
        protected abstract void CreateEventArgs(RenamedEventArgs rse, DataSourceChangeEventArgs<TKey> dsce);

        /// <summary>
        /// 監視待機中かどうかを示す値を取得します。
        /// </summary>
        public bool Waiting
        {
            get { return !m_watcher.EnableRaisingEvents; }
            private set { m_watcher.EnableRaisingEvents = !value; }
        }
  
        /// <summary>
        /// 監視するディレクトリのパスを取得します。
        /// </summary>
        public abstract string Path { get; }

        /// <summary>
        /// ディレクトリで監視するファイルのフィルターを取得します。
        /// </summary>
        public abstract string Filter { get; }

        /// <summary>
        /// 監視する種類を取得します。
        /// </summary>
        public abstract NotifyFilters NotifyFilter { get; }
    }
}
