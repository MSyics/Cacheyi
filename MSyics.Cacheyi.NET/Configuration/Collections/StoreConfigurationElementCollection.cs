/****************************************************************
© 2016 MSyics
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
****************************************************************/
using System;
using System.Linq;
using System.Configuration;

namespace MSyics.Cacheyi.Configuration
{
    /// <summary>
    /// Store 要素を格納する構成要素を表します。
    /// </summary>
    internal class StoreConfigurationElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new StoreConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((StoreConfigurationElement)element).StoreName;
        }

        public StoreConfigurationElement FindOrDefault(Func<StoreConfigurationElement, bool> predicate)
        {
            return this.Cast<StoreConfigurationElement>().FirstOrDefault(predicate);
        }
    }
}
