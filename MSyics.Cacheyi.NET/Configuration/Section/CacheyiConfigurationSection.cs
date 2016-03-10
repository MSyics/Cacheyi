/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Configuration;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// 構成ファイルの msyics/cache セクションを表します。
    /// </summary>
    internal class CacheyiConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// 構成セクションを示す定数です。
        /// </summary>
        public const string SectionName = "msyics/cacheyi";

        [ConfigurationProperty("centers")]
        [ConfigurationCollection(typeof(CenterConfigurationElementCollection), AddItemName = "center", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public CenterConfigurationElementCollection Centers
        {
            get { return (CenterConfigurationElementCollection)base["centers"]; }
        }

        //[ConfigurationProperty("performanceCounters")]
        //public bool? PerformanceCounters
        //{
        //    get { return (bool?)this["performanceCounters"]; }
        //    set { this["performanceCounters"] = value; }
        //}
    }
}
