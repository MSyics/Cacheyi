using System.Configuration;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// Cacheyi を構成するための拡張機能
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// 設定情報をアプリケーション構成ファイルから読み込みます。
        /// </summary>
        /// <param name="settings"></param>
        public static void Read(this ICacheStoreSettings settings)
        {
            var cacheSection = ConfigurationManager.GetSection(CacheyiConfigurationSection.SectionName) as CacheyiConfigurationSection;
            if (cacheSection == null) { return; }

            //if (cacheSection.PerformanceCounters.HasValue)
            //{
            //    Store.PerformanceCounters = cacheSection.PerformanceCounters.Value;
            //}

            var centerElement = cacheSection.Centers.FindOrDefault(x => x.Name == settings.CenterType.FullName);
            if (centerElement == null) { return; }
            //if (providerElement.PerformanceCounters.HasValue)
            //{
            //    Store.PerformanceCounters = providerElement.PerformanceCounters.Value;
            //}

            var storeElement = centerElement.Stores.FindOrDefault(x => x.StoreName == settings.StoreName);
            if(storeElement == null) { return; }

            settings.MaxCapacity = storeElement.MaxCapacity;
            settings.Timeout = storeElement.Timeout;
        }
    }
}
