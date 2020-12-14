using System;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// 保持する要素の設定を行います。
    /// </summary>
    public interface ICacheValueConfiguration<TKey, TValue>
    {
        /// <summary>
        /// 保持する要素を取得します。
        /// </summary>
        /// <param name="builder">指定したキーから保持する要素を取得する機能</param>
        void GetValue(Func<TKey, TValue> builder);
    }

    /// <summary>
    /// 保持する要素の設定を行います。
    /// </summary>
    public interface ICacheValueConfiguration<TKeyed, TKey, TValue>
    {
        /// <summary>
        /// 保持する要素を取得します。
        /// </summary>
        /// <param name="builder">指定したキーから保持する要素を取得する機能</param>
        void GetValue(Func<TKeyed, TValue> builder);
    }
}
