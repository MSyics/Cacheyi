/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System.Configuration;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// Center 要素を表します。
    /// </summary>
    internal class CenterConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("stores")]
        [ConfigurationCollection(typeof(StoreConfigurationElementCollection), AddItemName = "store", CollectionType = ConfigurationElementCollectionType.BasicMap)]
        public StoreConfigurationElementCollection Stores
        {
            get { return (StoreConfigurationElementCollection)base["stores"]; }
        }

        //[ConfigurationProperty("performanceCounters")]
        //public bool? PerformanceCounters
        //{
        //    get { return (bool?)this["performanceCounters"]; }
        //    set { this["performanceCounters"] = value; }
        //}
    }
}
