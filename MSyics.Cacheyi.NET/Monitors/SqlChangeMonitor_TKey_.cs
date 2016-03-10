/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Data.SqlClient;
using System.Threading;
using System.Collections.ObjectModel;

namespace MSyics.Cacheyi.Monitors
{
    /// <summary>
    /// <para>SQL Server 2005 over インスタンス間でのクエリ通知を実装するための基底クラスです。</para>
    /// <para>Service Broker を有効にしてください。</para>
    /// </summary>
    /// <typeparam name="TKey">キーの型</typeparam>
    public abstract class SqlChangeMonitor<TKey> : IDataSourceChangeMonitor<TKey>
    {
        private SqlDependency m_sqlDependency;

        /// <summary>
        /// SqlChangeMonitor&lt;TKey&gt; クラスのインスタンスを初期化します。
        /// </summary>
        /// <param name="connectionString">SQL Server インスタンスの接続文字列</param>
        public SqlChangeMonitor(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// SQL Server インスタンスの接続文字列を取得します。
        /// </summary>
        public string ConnectionString { get; private set; }

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

            this.Waiting = false;
            SqlDependency.Start(this.ConnectionString);
            Register();
        }

        /// <summary>
        /// データソースの変更通知を停止します。
        /// </summary>
        public void Stop()
        {
            if (this.Waiting) { return; }

            Unregister();
            SqlDependency.Stop(this.ConnectionString);

            this.Waiting = true;
        }

        private void Register()
        {
            using (var cnn = new SqlConnection(this.ConnectionString))
            using (var cmd = new SqlCommand(GetDependencyCommandText(), cnn))
            {
                Unregister();
                this.m_sqlDependency = new SqlDependency(cmd);
                this.m_sqlDependency.OnChange += new OnChangeEventHandler(Dependency_OnChange);

                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void Unregister()
        {
            if (this.m_sqlDependency != null)
            {
                this.m_sqlDependency.OnChange -= Dependency_OnChange;
            }
        }

        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (m_sqlDependency != null)
            {
                m_sqlDependency.OnChange -= new OnChangeEventHandler(Dependency_OnChange);
            }

            var receive = Interlocked.CompareExchange(ref OnChanged, null, null);
            if (receive != null)
            {
                var vce = new DataSourceChangeEventArgs<TKey>()
                {
                    ChangeAction = CacheChangeAction.None,
                    Keys = new Collection<TKey>(),
                };
                CreateEventArgs(e, vce);
                receive(this, vce);
                Register();
            }
        }

        /// <summary>
        /// 依存するクエリテキストを取得します。
        /// </summary>
        /// <returns>クエリテキスト</returns>
        protected abstract string GetDependencyCommandText();

        /// <summary>
        /// データソース変更時のイベントデータを作成します。
        /// </summary>
        /// <param name="sne">通知イベントの一連の引数</param>
        /// <param name="dsce">データソース変更時の一連のデータ</param>
        protected abstract void CreateEventArgs(SqlNotificationEventArgs sne, DataSourceChangeEventArgs<TKey> dsce);

        /// <summary>
        /// 監視待機中かどうかを示す値を取得します。
        /// </summary>
        public bool Waiting { get; private set; } = true;
    }
}

